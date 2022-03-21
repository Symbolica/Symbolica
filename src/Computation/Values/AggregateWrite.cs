using System;
using System.Collections;
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

internal sealed class AggregateWrite : BitVector, IComparable<AggregateWrite>
{
    private sealed class Fields : IEnumerable<IValue>
    {
        private readonly Bits _size;
        private readonly ImmutableList<AggregateWrite> _values;

        private Fields(Bits size, IValue mask, ImmutableList<AggregateWrite> values)
        {
            _size = size;
            Mask = mask;
            _values = values;
        }

        public IValue Data => _values.Aggregate(
                ConstantUnsigned.Zero(_size) as IValue,
                (acc, field) => Or.Create(acc, CreateData(field)));

        public IValue Mask { get; }

        public AggregateWrite this[int index] => _values[index];

        public int BinarySearch(AggregateWrite item)
        {
            return _values.BinarySearch(item);
        }

        public Fields Insert(int index, AggregateWrite field)
        {
            return new(_size, Or.Create(Mask, Mask(_size, field._offset, field.Size)), _values.Insert(index, field));
        }

        public Fields Replace(AggregateWrite oldValue, AggregateWrite newValue)
        {
            return new(_size, Mask, _values.Replace(oldValue, newValue));
        }

        public IEnumerator<IValue> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_values as IEnumerable).GetEnumerator();
        }

        private IValue CreateData(AggregateWrite field)
        {
            return ShiftLeft.Create(Resize(field.Flatten(), _size), Resize(field._offset, _size));
        }

        public static Fields Create(Bits size, AggregateWrite field)
        {
            return new Fields(size, Mask(size, field._offset, field.Size), ImmutableList.Create(field));
        }

        public static Fields Empty(Bits size)
        {
            return new Fields(size, ConstantUnsigned.Zero(size), ImmutableList.Create<AggregateWrite>());
        }
    }

    private readonly IValue _buffer;
    private readonly IValue _offset;
    private readonly BigInteger _rank;
    private readonly Fields _fields;

    private AggregateWrite(IValue buffer, IValue offset, BigInteger rank, Fields fields)
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

    public override BitVecExpr AsBitVector(IContext context)
    {
        return Flatten().AsBitVector(context);
    }

    public int CompareTo(AggregateWrite? other) => _rank.CompareTo(other?._rank);

    internal IValue Read(ICollectionFactory collectionFactory, IAssertions assertions,
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
                    assertions,
                    WriteOffsets.Create(ConstantUnsigned.Zero(Size), Size, valueSize),
                    valueSize)
                : _buffer;
        }

        WriteOffset offset = offsets.Head();
        if (Size != offset.AggregateSize)
            throw new InconsistentExpressionSizesException(Size, offset.AggregateSize);

        if (offset.FieldSize == Size && offset.Value is IConstantValue o && o.AsUnsigned().IsZero)
            return Read(collectionFactory, assertions, offsets.Tail(), valueSize);

        IValue ReadField(AggregateWrite field)
        {
            return field.Read(
                collectionFactory,
                assertions,
                offsets.Rebase(field.Size, field._offset),
                valueSize);
        }

        IValue OverlappingRead()
        {
            // This could be smarter, just because something overlaps at this layer, it doesn't mean it won't at a lower
            // level, e.g. reading a field from a struct within an array with a symbolic index
            return Values.Read.Create(collectionFactory, assertions, Flatten(), offsets, valueSize);
        }

        var result = _fields.BinarySearch(CreateLeaf(assertions, offset, ConstantUnsigned.Zero(offset.FieldSize)));
        if (result < 0)
        {
            var index = ~result;
            // No exact match, index points to the next element that is bigger than this one,
            // we're going to be naive and assume everything is fully contained (evidence so far suggests it is)
            // in which case we only need to check the one below here because the one with a greater index clearly
            // cant fully contain this offset
            var lower = index > 0 ? _fields[index - 1] : null;
            if (lower is not null && offset.IsBoundedBy(assertions, lower._offset, lower.Size))
                return ReadField(lower);

            if (!offset.IsBoundedBy(assertions, ConstantUnsigned.Zero(_offset.Size), Size))
                throw new Exception("Oh no, looks like we're going to have to try and read from a higher level where this does fit.");

            return IsNotOverlappingAnyField(assertions, offset)
                ? Values.Read.Create(collectionFactory, assertions, _buffer, offsets, valueSize)
                : OverlappingRead();
        }
        else
        {
            var match = _fields[result];
            if (offset.IsBoundedBy(assertions, match._offset, match.Size))
                return ReadField(match);

            if (!offset.IsBoundedBy(assertions, ConstantUnsigned.Zero(_offset.Size), Size))
                throw new Exception("Oh no, looks like we're going to have to try and read from a higher level where this does fit.");

            return OverlappingRead();
        }
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

            return new AggregateWrite(value, _offset, _rank, Fields.Empty(Size));
        }

        WriteOffset offset = offsets.Head();
        if (offset.AggregateSize != Size)
            throw new InconsistentExpressionSizesException(offset.AggregateSize, Size);

        if (offset.FieldSize == Size && offset.Value is IConstantValue o && o.AsUnsigned().IsZero)
            return Write(collectionFactory, assertions, offsets.Tail(), value);

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
                        assertions,
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
                Fields.Create(Size, CreateField(collectionFactory, assertions, this, offsets, value)));
        }

        var result = _fields.BinarySearch(CreateLeaf(assertions, offset, ConstantUnsigned.Zero(offset.FieldSize)));
        if (result < 0)
        {
            var index = ~result;
            // No exact match, index points to the next element that is bigger than this one,
            // we're going to be naive and assume everything is fully contained (evidence so far suggests it is)
            // in which case we only need to check the one below here because the one with a greater index clearly
            // cant fully contain this offset
            var lower = index > 0 ? _fields[index - 1] : null;
            if (lower is not null && offset.IsBoundedBy(assertions, lower._offset, lower.Size))
                return WriteField(lower);

            if (!offset.IsBoundedBy(assertions, ConstantUnsigned.Zero(_offset.Size), Size))
                throw new Exception("Oh no, looks like we're going to have to try and read from a higher level where this does fit.");

            return IsNotOverlappingAnyField(assertions, offset)
                ? new AggregateWrite(
                    _buffer,
                    _offset,
                    _rank,
                    _fields.Insert(index, CreateField(collectionFactory, assertions, _buffer, offsets, value)))
                : OverlappingWrite();
        }
        else
        {
            var match = _fields[result];
            if (offset.IsBoundedBy(assertions, match._offset, match.Size))
                return WriteField(match);

            if (!offset.IsBoundedBy(assertions, ConstantUnsigned.Zero(_offset.Size), Size))
                throw new Exception("Oh no, looks like we're going to have to try and read from a higher level where this does fit.");

            return OverlappingWrite();
        }
    }

    private static IValue Mask(Bits bufferSize, IValue offset, Bits size)
    {
        return ShiftLeft.Create(
            ConstantUnsigned.Zero(size).Not().Extend(bufferSize),
            Resize(offset, bufferSize));
    }

    private static IValue Resize(IValue value, Bits size) =>
        Truncate.Create(size, ZeroExtend.Create(size, value));

    private bool IsNotOverlappingAnyField(IAssertions assertions, WriteOffset offset)
    {
        var isOverlapping = And.Create(_fields.Mask, Mask(Size, offset.Value, offset.FieldSize));
        using var proposition = assertions.GetProposition(isOverlapping);

        return !proposition.CanBeTrue;
    }

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

        return Or.Create(And.Create(_buffer, Not.Create(_fields.Mask)), _fields.Data);
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
                    CreateInitial(assertions, buffer, offset.Size),
                    offset,
                    value);
    }

    private static AggregateWrite CreateInitial(IAssertions assertions, IValue buffer, Bits offsetSize)
    {
        return CreateLeaf(
            assertions,
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
            ? CreateLeaf(assertions, offset, value)
            : CreateLeaf(
                assertions,
                offset,
                Values.Read.Create(collectionFactory, assertions, buffer, offset.Value, offset.FieldSize))
                .Write(collectionFactory, assertions, tail, value);
    }

    private static AggregateWrite CreateLeaf(IAssertions assertions, WriteOffset offset, IValue value)
    {
        if (value.Size != offset.FieldSize)
            throw new InconsistentExpressionSizesException(value.Size, offset.FieldSize);

        return new AggregateWrite(
            value,
            offset.Value,
            assertions.GetValue(offset.Value).AsUnsigned(),
            Fields.Empty(value.Size));
    }
}
