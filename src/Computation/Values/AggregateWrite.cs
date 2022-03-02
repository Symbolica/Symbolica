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

internal sealed record AggregateWrite : BitVector
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

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        return Flatten().AsBitVector(solver);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as AggregateWrite);
    }

    internal IValue Read(ICollectionFactory collectionFactory, ISolver solver,
        WriteOffsets offsets, Bits valueSize)
    {
        if (offsets.Empty)
        {
            // We've hit the field that the value should be read from
            if (valueSize != Size)
                throw new InconsistentExpressionSizesException(Size, valueSize);

            // In the trivial case (e.g. when this AggregateWrite is terminal)
            // then flatten is just the same as returning the buffer.
            return Flatten();
        }

        WriteOffset offset = offsets.Head();
        if (Size != offset.AggregateSize)
            throw new InconsistentExpressionSizesException(Size, offset.AggregateSize);

        IValue ReadField(AggregateWrite field)
        {
            return field.Read(collectionFactory, solver, offsets.Tail(), valueSize);
        }

        var constantMatch = _fields
            .Where(f => f._offset is IConstantValue)
            .FirstOrDefault(f => f.IsAligned(solver, offset));
        if (constantMatch is not null)
        {
            return ReadField(constantMatch);
        }

        if (IsNotOverlappingAnyField(solver, offset))
        {
            return Values.Read.Create(collectionFactory, solver, _buffer, offsets, valueSize);
        }

        var symbolicMatch = _fields
            .Where(f => f._offset is not IConstantValue)
            .FirstOrDefault(f => f.IsAligned(solver, offset));
        if (symbolicMatch is not null)
        {
            return ReadField(symbolicMatch);
        }

        // Just because we have missed it doesn't mean we need to flatten.
        // E.g. consider MemorySet where it might have just set some sub section and so created an artificial
        // field that spans multiple actual fields.
        // So we should either find a way to make MemorySet target the specific struct fields
        // or we should be more sophisticated here and check if this will fit inside an existing field (with correct alignment)
        // and then write it into there instead
        var flattened = Flatten();
        return Values.Read.Create(
            collectionFactory,
            solver,
            Flatten(),
            offsets.Aggregate(),
            valueSize);
    }

    internal AggregateWrite Write(ICollectionFactory collectionFactory, ISolver solver,
        WriteOffsets offsets, IValue value)
    {
        if (offsets.Empty)
        {
            // We've hit the field that the value should be written to
            if (Size != value.Size)
                throw new InconsistentExpressionSizesException(Size, value.Size);

            return new AggregateWrite(value, _offset, ImmutableList.Create<AggregateWrite>());
        }

        WriteOffset offset = offsets.Head();
        if (Size != offset.AggregateSize)
            throw new InconsistentExpressionSizesException(Size, offset.AggregateSize);

        AggregateWrite WriteField(AggregateWrite field)
        {
            return new AggregateWrite(
                _buffer,
                _offset,
                _fields.Replace(
                    field,
                    field.Write(collectionFactory, solver, offsets.Tail(), value)));
        }

        var constantMatch = _fields
            .Where(f => f._offset is IConstantValue)
            .FirstOrDefault(f => f.IsAligned(solver, offset));
        if (constantMatch is not null)
        {
            return WriteField(constantMatch);
        }

        if (IsNotOverlappingAnyField(solver, offset))
        {
            return new AggregateWrite(
                _buffer,
                _offset,
                _fields.Add(CreateField(collectionFactory, solver, _buffer, offsets, value)));
        }

        var symbolicMatch = _fields
            .Where(f => f._offset is not IConstantValue)
            .FirstOrDefault(f => f.IsAligned(solver, offset));
        if (symbolicMatch is not null)
        {
            return WriteField(symbolicMatch);
        }

        var flattened = Flatten();
        return new AggregateWrite(
            flattened,
            _offset,
            ImmutableList.Create(CreateField(collectionFactory, solver, flattened, offsets, value)));
    }

    private static IValue Mask(Bits bufferSize, IValue offset, Bits size)
    {
        return ShiftLeft.Create(
            ConstantUnsigned.Create(size, BigInteger.Zero).Not().Extend(bufferSize),
            ZeroExtend.Create(bufferSize, offset));
    }

    private bool IsNotOverlappingAnyField(ISolver solver, WriteOffset offset)
    {
        var isOverlapping = And.Create(AggregateMask(), Mask(Size, offset.Value, offset.FieldSize));
        return !solver.IsSatisfiable(isOverlapping);
    }

    private bool IsAligned(ISolver solver, WriteOffset offset)
    {
        if (Size != offset.FieldSize)
        {
            return false;
        }
        var isNotAligned = NotEqual.Create(_offset, offset.Value);
        return !solver.IsSatisfiable(isNotAligned);
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

    public static IValue Create(ICollectionFactory collectionFactory, ISolver solver,
        IValue buffer, IValue offset, IValue value)
    {
        if (offset is Address<Bits> x && x.Aggregate() is IConstantValue y && y.AsUnsigned() == (BigInteger) 1920)
            Debugger.Break();

        return buffer is IConstantValue b && offset is IConstantValue o && value is IConstantValue v
            ? b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory))
            : buffer is AggregateWrite w
                ? w.Write(
                    collectionFactory,
                    solver,
                    WriteOffsets.Create(solver, offset, buffer.Size, value.Size),
                    value)
                : Create(
                    collectionFactory,
                    solver,
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

    private static AggregateWrite CreateField(ICollectionFactory collectionFactory, ISolver solver,
        IValue buffer, WriteOffsets offsets, IValue value)
    {
        var offset = offsets.Head();
        if (buffer.Size != offset.AggregateSize)
            throw new InconsistentExpressionSizesException(buffer.Size, offset.AggregateSize);

        var fieldBuffer = Values.Read.Create(collectionFactory, solver, buffer, offset.Value, offset.FieldSize);

        var tail = offsets.Tail();
        return tail.Empty
            ? CreateLeaf(offset, value)
            : new AggregateWrite(
                fieldBuffer,
                offset.Value,
                ImmutableList.Create(CreateField(collectionFactory, solver, fieldBuffer, tail, value)));
    }

    private static AggregateWrite CreateLeaf(WriteOffset offset, IValue value)
    {
        if (value.Size != offset.FieldSize)
            throw new InconsistentExpressionSizesException(value.Size, offset.FieldSize);

        // TODO: Review this ZeroExtend
        return new AggregateWrite(
            value,
            offset.Value,
            ImmutableList.Create<AggregateWrite>());
    }
}
