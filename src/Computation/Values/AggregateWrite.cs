using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Exceptions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record AggregateWrite : BitVector, IComparable<AggregateWrite>
{
    private readonly IValue _buffer;
    private readonly IValue _offset;
    private readonly BigInteger _rank;
    private readonly ImmutableList<AggregateWrite> _fields;

    private AggregateWrite(IValue buffer, IValue offset, BigInteger rank, ImmutableList<AggregateWrite> fields)
        : base(buffer.Size)
    {
        if (rank < 0)
            Debugger.Break();

        _buffer = buffer;
        _offset = offset;
        _rank = rank;
        _fields = fields;
    }

    public override IEnumerable<IValue> Children => new[] { _buffer }.Concat(_fields);

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        return Flatten().AsBitVector(solver);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as AggregateWrite);
    }

    public int CompareTo(AggregateWrite? other) => _rank.CompareTo(other?._rank);

    internal IValue Read(ICollectionFactory collectionFactory, ISolver solver,
        WriteOffsets offsets, Bits valueSize)
    {
        if (offsets.Empty)
        {
            // We've hit the field that the value should be read from
            if (valueSize > Size)
                throw new InconsistentExpressionSizesException(Size, valueSize);

            return valueSize < Size
                // Create an offset whose field size is the same as the value size and write back to this same layer
                // so that it will either create a new field at offset zero of that size, or find an existing field
                // which is at least that big and write into that
                ? Read(
                    collectionFactory,
                    solver,
                    WriteOffsets.Create(ConstantUnsigned.CreateZero(Size), Size, valueSize),
                    valueSize)
                : _buffer;
        }

        WriteOffset offset = offsets.Head();
        if (Size != offset.AggregateSize)
            throw new InconsistentExpressionSizesException(Size, offset.AggregateSize);

        if (offset.FieldSize == Size && offset.Value is IConstantValue o && o.AsUnsigned().IsZero)
            return Read(collectionFactory, solver, offsets.Tail(), valueSize);

        IValue ReadField(AggregateWrite field)
        {
            return field.Read(
                collectionFactory,
                solver,
                offsets.Rebase(field.Size, field._offset),
                valueSize);
        }

        IValue OverlappingRead()
        {
            // This could be smarter, just because something overlaps at this layer, it doesn't mean it won't at a lower
            // level, e.g. reading a field from a struct within an array with a symbolic index
            return Values.Read.Create(collectionFactory, solver, Flatten(), offsets, valueSize);
        }

        var result = _fields.BinarySearch(CreateLeaf(solver, offset, ConstantUnsigned.CreateZero(offset.FieldSize)));
        if (result < 0)
        {
            var index = ~result;
            // No exact match, index points to the next element that is bigger than this one,
            // we're going to be naive and assume everything is fully contained (evidence so far suggests it is)
            // in which case we only need to check the one below here because the one with a greater index clearly
            // cant fully contain this offset
            var lower = index > 0 ? _fields[index - 1] : null;
            if (lower is not null && offset.IsBoundedBy(solver, lower._offset, lower.Size))
                return ReadField(lower);

            if (!offset.IsBoundedBy(solver, ConstantUnsigned.CreateZero(_offset.Size), Size))
                throw new Exception("Oh no, looks like we're going to have to try and read from a higher level where this does fit.");

            return IsNotOverlappingAnyField(solver, offset)
                ? Values.Read.Create(collectionFactory, solver, _buffer, offsets, valueSize)
                : OverlappingRead();
        }
        else
        {
            var match = _fields[result];
            if (offset.IsBoundedBy(solver, match._offset, match.Size))
                return ReadField(match);

            if (!offset.IsBoundedBy(solver, ConstantUnsigned.CreateZero(_offset.Size), Size))
                throw new Exception("Oh no, looks like we're going to have to try and read from a higher level where this does fit.");

            return OverlappingRead();
        }
    }

    internal AggregateWrite Write(ICollectionFactory collectionFactory, ISolver solver,
        WriteOffsets offsets, IValue value)
    {
        if (offsets.Empty)
        {
            // Don't expect this, but maybe if it does happen it should return null from here and overwrite
            // at a higher (wider) level
            if (value.Size > Size)
                throw new InconsistentExpressionSizesException(Size, value.Size);

            if (value.Size < Size)
                // Create an offset whose field size is the same as the value size and write back to this same layer
                // so that it will either create a new field at offset zero of that size, or find an existing field
                // which is at least that big and write into that
                return Write(
                    collectionFactory,
                    solver,
                    WriteOffsets.Create(ConstantUnsigned.CreateZero(Size), Size, value.Size),
                    value);

            return new AggregateWrite(value, _offset, _rank, ImmutableList.Create<AggregateWrite>());
        }

        WriteOffset offset = offsets.Head();
        if (offset.AggregateSize != Size)
            throw new InconsistentExpressionSizesException(offset.AggregateSize, Size);

        if (offset.FieldSize == Size && offset.Value is IConstantValue o && o.AsUnsigned().IsZero)
            return Write(collectionFactory, solver, offsets.Tail(), value);

        AggregateWrite WriteField(AggregateWrite field)
        {
            return new AggregateWrite(
                _buffer,
                _offset,
                _rank,
                _fields.Replace(
                    field,
                    field.Write(
                        collectionFactory,
                        solver,
                        offsets.Rebase(field.Size, field._offset),
                        value)));
        }

        AggregateWrite OverlappingWrite()
        {
            // It is within this level but it overlaps with other fields
            // Create a new layer at this level with the existing level as its value.
            // Then when we want to read if we find the offset is disjoint then we can just read the layer below
            // if it within a field then it can read from there knowing that its backing value will be the subsection of the underlying Write
            // and if its overlapping then it will have to finally flatten
            return new AggregateWrite(
                this,
                _offset,
                _rank,
                ImmutableList.Create(CreateField(collectionFactory, solver, this, offsets, value)));
        }

        var result = _fields.BinarySearch(CreateLeaf(solver, offset, ConstantUnsigned.CreateZero(offset.FieldSize)));
        if (result < 0)
        {
            var index = ~result;
            // No exact match, index points to the next element that is bigger than this one,
            // we're going to be naive and assume everything is fully contained (evidence so far suggests it is)
            // in which case we only need to check the one below here because the one with a greater index clearly
            // cant fully contain this offset
            var lower = index > 0 ? _fields[index - 1] : null;
            if (lower is not null && offset.IsBoundedBy(solver, lower._offset, lower.Size))
                return WriteField(lower);

            if (!offset.IsBoundedBy(solver, ConstantUnsigned.CreateZero(_offset.Size), Size))
                throw new Exception("Oh no, looks like we're going to have to try and read from a higher level where this does fit.");

            return IsNotOverlappingAnyField(solver, offset)
                ? new AggregateWrite(
                    _buffer,
                    _offset,
                    _rank,
                    _fields.Insert(index, CreateField(collectionFactory, solver, _buffer, offsets, value)))
                : OverlappingWrite();
        }
        else
        {
            var match = _fields[result];
            if (offset.IsBoundedBy(solver, match._offset, match.Size))
                return WriteField(match);

            if (!offset.IsBoundedBy(solver, ConstantUnsigned.CreateZero(_offset.Size), Size))
                throw new Exception("Oh no, looks like we're going to have to try and read from a higher level where this does fit.");

            return OverlappingWrite();
        }
    }

    private static IValue Mask(Bits bufferSize, IValue offset, Bits size)
    {
        return ShiftLeft.Create(
            ConstantUnsigned.Create(size, BigInteger.Zero).Not().Extend(bufferSize),
            Resize(offset, bufferSize));
    }

    private static IValue Resize(IValue value, Bits size) =>
        Truncate.Create(size, ZeroExtend.Create(size, value));

    private bool IsNotOverlappingAnyField(ISolver solver, WriteOffset offset)
    {
        var isOverlapping = And.Create(AggregateMask(), Mask(Size, offset.Value, offset.FieldSize));
        return !solver.IsSatisfiable(isOverlapping);
    }

    private IValue AggregateFields(Func<AggregateWrite, IValue> f)
    {
        return _fields.Aggregate(
            ConstantUnsigned.Create(Size, BigInteger.Zero) as IValue,
            (acc, field) => Or.Create(acc, f(field)));
    }

    private IValue AggregateMask() => AggregateFields(f => Mask(Size, f._offset, f.Size));

    private IValue Flatten()
    {
        // TODO: Try to separate the rolling up of the values from the application to
        // the underlying buffer so that we can avoid duplication of the underlying.
        // Imagine the case where the underlying is symbolic and we've created sub-portions
        // of it at each layer. Then rolling them all back up could include many copies
        // of this complex expression.
        // Instead, if we keep every single write (even constants) in a layer and then
        // only merge those together for a given a layer, the caller of this can then
        // apply the result to their copy of the underlying which will mean we only see
        // a single instance of it but with a minimised size still.
        IValue CreateData(AggregateWrite field)
        {
            return ShiftLeft.Create(Resize(field.Flatten(), Size), Resize(field._offset, Size));
        }

        var data = AggregateFields(CreateData);

        return Or.Create(And.Create(_buffer, Not.Create(AggregateMask())), data);
    }

    public static IValue Create(ICollectionFactory collectionFactory, ISolver solver,
        IValue buffer, IValue offset, IValue value)
    {
        return buffer is IConstantValue b && offset is IConstantValue o && value is IConstantValue v
            ? b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory))
            : buffer is AggregateWrite w
                ? w.Write(
                    collectionFactory,
                    solver,
                    WriteOffsets.Create(offset, buffer.Size, value.Size),
                    value)
                : Create(
                    collectionFactory,
                    solver,
                    CreateInitial(solver, buffer, offset.Size),
                    offset,
                    value);
    }

    private static AggregateWrite CreateInitial(ISolver solver, IValue buffer, Bits offsetSize)
    {
        return CreateLeaf(
            solver,
            new WriteOffset(
                buffer.Size,
                "Base Write",
                buffer.Size,
                ConstantUnsigned.Create(offsetSize, BigInteger.Zero)),
            buffer);
    }

    private static AggregateWrite CreateField(ICollectionFactory collectionFactory, ISolver solver,
        IValue buffer, WriteOffsets offsets, IValue value)
    {
        var offset = offsets.Head();
        if (buffer.Size != offset.AggregateSize)
            throw new InconsistentExpressionSizesException(buffer.Size, offset.AggregateSize);

        var tail = offsets.Tail();
        return tail.Empty
            ? CreateLeaf(solver, offset, value)
            : CreateLeaf(
                solver,
                offset,
                Values.Read.Create(collectionFactory, solver, buffer, offset.Value, offset.FieldSize))
                .Write(collectionFactory, solver, tail, value);
    }

    private static AggregateWrite CreateLeaf(ISolver solver, WriteOffset offset, IValue value)
    {
        if (value.Size != offset.FieldSize)
            throw new InconsistentExpressionSizesException(value.Size, offset.FieldSize);

        return new AggregateWrite(
            value,
            offset.Value,
            solver.GetExampleValue(offset.Value),
            ImmutableList.Create<AggregateWrite>());
    }
}
