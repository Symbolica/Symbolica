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

internal sealed class AggregateWrite : BitVector
{
    private readonly IValue _buffer;
    private readonly IValue _offset;
    private readonly ImmutableList<AggregateWrite> _fields;

    private AggregateWrite(IValue buffer, IValue offset, ImmutableList<AggregateWrite> fields)
        : base(buffer.Size)
    {
        if (!(buffer is IConstantValue || buffer is Address<Bytes>))
            Debugger.Break();

        _buffer = buffer;
        _offset = offset;
        _fields = fields;
    }

    public override IEnumerable<IValue> Children => new[] { _buffer }.Concat(_fields);

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(IContext context)
    {
        return Flatten().AsBitVector(context);
    }

    internal IValue Read(ICollectionFactory collectionFactory, IAssertions assertions,
        WriteOffsets offsets, Bits valueSize)
    {
        if (offsets.Empty)
        {
            // We've hit the field that the value should be read from
            if (valueSize > Size)
                throw new InconsistentExpressionSizesException(Size, valueSize);

            if (valueSize < Size)
                // Create an offset whose field size is the same as the value size and write back to this same layer
                // so that it will either create a new field at offset zero of that size, or find an existing field
                // which is at least that big and write into that
                return Read(
                    collectionFactory,
                    assertions,
                    WriteOffsets.Create(ConstantUnsigned.Zero(Size), Size, valueSize),
                    valueSize);

            // In the trivial case (e.g. when this AggregateWrite is terminal)
            // then flatten is just the same as returning the buffer.
            return _buffer;
        }

        WriteOffset offset = offsets.Head();
        if (Size != offset.AggregateSize)
            throw new InconsistentExpressionSizesException(Size, offset.AggregateSize);

        if (offset.FieldSize == Size && offset.IsZero(assertions))
            return Read(collectionFactory, assertions, offsets.Tail(), valueSize);

        IValue ReadField(AggregateWrite field)
        {
            return field.Read(
                collectionFactory,
                assertions,
                offsets.Rebase(field.Size, field._offset),
                valueSize);
        }

        // Can we find a field that contains this offset?
        var constantMatch = _fields
            .Where(f => f._offset is IConstantValue)
            .FirstOrDefault(f => offset.IsBoundedBy(assertions, f._offset, f.Size));
        if (constantMatch is not null)
            return ReadField(constantMatch);

        // If the offset isn't within an existing field then let's at least make sure it's within the bounds of this level
        if (!offset.IsBoundedBy(assertions, ConstantUnsigned.Zero(_offset.Size), Size))
            throw new Exception("Oh no, looks like we're going to have to try and read from a higher level where this does fit.");

        if (IsNotOverlappingAnyField(assertions, offset))
            return Values.Read.Create(collectionFactory, assertions, _buffer, offsets, valueSize);

        var symbolicMatch = _fields
            .Where(f => f._offset is not IConstantValue)
            .FirstOrDefault(f => offset.IsBoundedBy(assertions, f._offset, f.Size));
        if (symbolicMatch is not null)
            return ReadField(symbolicMatch);

        // This could be smarter, just because something overlaps at this layer, it doesn't mean it won't at a lower
        // level, e.g. reading a field from a struct within an array with a symbolic index
        return Values.Read.Create(collectionFactory, assertions, Flatten(), offsets, valueSize);
    }

    internal AggregateWrite Write(ICollectionFactory collectionFactory, IAssertions assertions,
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
                    assertions,
                    WriteOffsets.Create(ConstantUnsigned.Zero(Size), Size, value.Size),
                    value);

            return new AggregateWrite(value, _offset, ImmutableList.Create<AggregateWrite>());
        }

        WriteOffset offset = offsets.Head();
        if (offset.AggregateSize != Size)
            throw new InconsistentExpressionSizesException(offset.AggregateSize, Size);

        if (offset.FieldSize == Size && offset.IsZero(assertions))
            return Write(collectionFactory, assertions, offsets.Tail(), value);

        AggregateWrite WriteField(AggregateWrite field)
        {
            // TODO: Remove the matches offset from this offset and go again, to cater for when it's not an exact alignment
            // e.g. rogue pointers
            return new AggregateWrite(
                _buffer,
                _offset,
                _fields.Replace(
                    field,
                    field.Write(
                        collectionFactory,
                        assertions,
                        offsets.Rebase(field.Size, field._offset),
                        value)));
        }

        // Can we find a field that contains this offset?
        var constantMatch = _fields
            .Where(f => f._offset is IConstantValue)
            .FirstOrDefault(f => offset.IsBoundedBy(assertions, f._offset, f.Size));
        if (constantMatch is not null)
        {
            return WriteField(constantMatch);
        }

        // If the offset isn't within an existing field then let's at least make sure it's within the bounds of this level
        if (!offset.IsBoundedBy(assertions, ConstantUnsigned.Zero(_offset.Size), Size))
            throw new Exception("Oh no, looks like we're going to have to try and write to a higher level where this does fit.");

        if (IsNotOverlappingAnyField(assertions, offset))
        {
            return new AggregateWrite(
                _buffer,
                _offset,
                _fields.Add(CreateField(collectionFactory, assertions, _buffer, offsets, value)));
        }

        var symbolicMatch = _fields
            .Where(f => f._offset is not IConstantValue)
            .FirstOrDefault(f => offset.IsBoundedBy(assertions, f._offset, f.Size));
        if (symbolicMatch is not null)
        {
            return WriteField(symbolicMatch);
        }

        // It is within this level but it overlaps with other fields
        // Create a new layer at this level with the existing level as its value.
        // Then when we want to read if we find the offset is disjoint then we can just read the layer below
        // if it within a field then it can read from there knowing that its backing value will be the subsection of the underlying Write
        // and if its overlapping then it will have to finally flatten
        return new AggregateWrite(
            this,
            _offset,
            ImmutableList.Create(CreateField(collectionFactory, assertions, this, offsets, value)));
    }

    private static IValue Mask(Bits bufferSize, IValue offset, Bits size)
    {
        return ShiftLeft.Create(
            ConstantUnsigned.Create(size, BigInteger.Zero).Not().Extend(bufferSize),
            ZeroExtend.Create(bufferSize, offset));
    }

    private bool IsNotOverlappingAnyField(IAssertions assertions, WriteOffset offset)
    {
        var isOverlapping = And.Create(AggregateMask(), Mask(Size, offset.Value, offset.FieldSize));
        using var proposition = assertions.GetProposition(isOverlapping);

        return !proposition.CanBeTrue;
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
            return ShiftLeft.Create(
                ZeroExtend.Create(Size, field.Flatten()),
                ZeroExtend.Create(Size, field._offset));
        }

        var data = AggregateFields(CreateData);

        return Or.Create(And.Create(_buffer, Not.Create(AggregateMask())), data);
    }

    public static IValue Create(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue buffer, IValue offset, IValue value)
    {
        return buffer is IConstantValue b && offset is IConstantValue o && value is IConstantValue v
            ? b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory))
            : buffer is AggregateWrite w
                ? w.Write(
                    collectionFactory,
                    assertions,
                    WriteOffsets.Create(offset, buffer.Size, value.Size),
                    value)
                : Create(
                    collectionFactory,
                    assertions,
                    CreateInitial(buffer, offset.Size),
                    offset,
                    value);
    }

    private static AggregateWrite CreateInitial(IValue buffer, Bits offsetSize)
    {
        return CreateLeaf(
            new WriteOffset(
                buffer.Size,
                "Base Write",
                buffer.Size,
                ConstantUnsigned.Create(offsetSize, BigInteger.Zero)),
            buffer);
    }

    private static AggregateWrite CreateField(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue buffer, WriteOffsets offsets, IValue value)
    {
        var offset = offsets.Head();
        if (buffer.Size != offset.AggregateSize)
            throw new InconsistentExpressionSizesException(buffer.Size, offset.AggregateSize);

        var tail = offsets.Tail();
        return tail.Empty
            ? CreateLeaf(offset, value)
            : CreateLeaf(
                offset,
                Values.Read.Create(collectionFactory, assertions, buffer, offset.Value, offset.FieldSize))
                .Write(collectionFactory, assertions, tail, value);
    }

    private static AggregateWrite CreateLeaf(WriteOffset offset, IValue value)
    {
        if (value.Size != offset.FieldSize)
            throw new InconsistentExpressionSizesException(value.Size, offset.FieldSize);

        return new AggregateWrite(
            value,
            offset.Value,
            ImmutableList.Create<AggregateWrite>());
    }
}
