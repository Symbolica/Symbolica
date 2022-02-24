using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        AggregateOffset aggregateOffset, Bits valueSize)
    {
        if (Size != (Bits) (uint) aggregateOffset.AggregateSize)
            throw new InconsistentExpressionSizesException(Size, aggregateOffset.Size);

        var fieldOffset = aggregateOffset.Offset;
        var fieldSize = (Bits) (uint) (aggregateOffset.GetNext()?.AggregateSize ?? (uint) valueSize);

        IValue ReadFieldBuffer(IValue buffer)
        {
            return Values.Read.Create(
                collectionFactory,
                solver,
                buffer,
                ConstantUnsigned.Create(buffer.Size, 0),
                valueSize);
        }

        IValue ReadSubAggregate(AggregateWrite match)
        {
            var nextOffset = aggregateOffset.GetNext();
            return nextOffset is not null
                ? match.Read(collectionFactory, solver, nextOffset, valueSize)
                : ReadFieldBuffer(match.Flatten());
        }

        // Check for alignment with constant offsets
        //      Read sub aggregate
        var constantMatch = _fields
            .Where(f => f._offset is IConstantValue)
            .FirstOrDefault(f => f.IsAligned(solver, fieldSize, fieldOffset));
        if (constantMatch is not null)
        {
            return ReadSubAggregate(constantMatch);
        }

        // Check for disjunction with everything
        //      Read below
        if (IsNotOverlappingAnyField(solver, fieldSize, fieldOffset))
        {
            return Values.Read.Create(collectionFactory, solver, _buffer, fieldOffset, fieldSize);
        }

        // Check for alignment with symbolic offsets
        //      Do an overwrite
        var symbolicMatch = _fields
            .Where(f => f._offset is not IConstantValue)
            .FirstOrDefault(f => f.IsAligned(solver, fieldSize, fieldOffset));
        if (symbolicMatch is not null)
        {
            return ReadSubAggregate(symbolicMatch);
        }

        // Flatten
        return Values.Read.Create(collectionFactory, solver, Flatten(), fieldOffset, valueSize);
    }

    // Assumes that aggregateOffset has already been checked to make sure all offset are in bounds
    internal AggregateWrite Write(ICollectionFactory collectionFactory, ISolver solver, AggregateOffset aggregateOffset, IValue value)
    {
        if (Size != (Bits) (uint) aggregateOffset.AggregateSize)
            throw new InconsistentExpressionSizesException(Size, aggregateOffset.Size);

        var fieldOffset = aggregateOffset.Offset;
        var fieldSize = (Bits) (uint) (aggregateOffset.GetNext()?.AggregateSize ?? (uint) value.Size);

        AggregateWrite Overwrite(AggregateWrite match)
        {
            var nextOffset = aggregateOffset.GetNext();
            var newField = nextOffset is not null
                ? match.Write(collectionFactory, solver, nextOffset, value)
                : CreateLeaf(collectionFactory, solver, match, fieldOffset, value);
            return new AggregateWrite(_buffer, _offset, _fields.Replace(match, newField));
        }

        // Check for alignment with constant offsets
        //      Do an overwrite
        var constantMatch = _fields
            .Where(f => f._offset is IConstantValue)
            .FirstOrDefault(f => f.IsAligned(solver, fieldSize, fieldOffset));
        if (constantMatch is not null)
        {
            return Overwrite(constantMatch);
        }

        // Check for disjunction with everything
        //      Do an insert
        if (IsNotOverlappingAnyField(solver, fieldSize, fieldOffset))
        {
            return new AggregateWrite(
                _buffer,
                _offset,
                _fields.Add(CreateField(collectionFactory, solver, _buffer, aggregateOffset, value)));
        }

        // Check for alignment with symbolic offsets
        //      Do an overwrite
        var symbolicMatch = _fields
            .Where(f => f._offset is not IConstantValue)
            .FirstOrDefault(f => f.IsAligned(solver, fieldSize, fieldOffset));
        if (symbolicMatch is not null)
        {
            return Overwrite(symbolicMatch);
        }

        // Flatten
        var flattened = Flatten();
        return new AggregateWrite(
            flattened,
            fieldOffset,
            ImmutableList.Create(CreateField(collectionFactory, solver, flattened, aggregateOffset, value)));
    }

    private static IValue Mask(Bits bufferSize, IValue offset, Bits size)
    {
        return ShiftLeft.Create(
            ConstantUnsigned.Create(size, BigInteger.Zero).Not().Extend(bufferSize),
            ZeroExtend.Create(bufferSize, offset));
    }

    private bool IsNotOverlappingAnyField(ISolver solver, Bits size, IValue offset)
    {
        var isOverlapping = And.Create(AggregateMask(), Mask(Size, offset, size));
        return !solver.IsSatisfiable(isOverlapping);
    }

    private bool IsAligned(ISolver solver, Bits size, IValue offset)
    {
        if (Size != size)
        {
            return false;
        }
        var isNotAligned = NotEqual.Create(_offset, offset);
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
                ZeroExtend.Create(Size, _offset));
        }

        var data = AggregateFields(CreateData);

        return Or.Create(And.Create(_buffer, Not.Create(AggregateMask())), data);
    }

    public static AggregateWrite Create(ICollectionFactory collectionFactory, ISolver solver,
        IValue buffer, AggregateOffset aggregateOffset, IValue value)
    {
        if (buffer.Size != (Bits) (uint) aggregateOffset.AggregateSize)
            throw new InconsistentExpressionSizesException(buffer.Size, aggregateOffset.Size);

        return new AggregateWrite(
            buffer,
            ConstantUnsigned.Create(aggregateOffset.Size, BigInteger.Zero),
            ImmutableList.Create(CreateField(collectionFactory, solver, buffer, aggregateOffset, value)));
    }

    private static AggregateWrite CreateField(ICollectionFactory collectionFactory, ISolver solver,
        IValue buffer, AggregateOffset aggregateOffset, IValue value)
    {
        if (buffer.Size != (Bits) (uint) aggregateOffset.AggregateSize)
            throw new InconsistentExpressionSizesException(buffer.Size, aggregateOffset.Size);

        var nextOffset = aggregateOffset.GetNext();
        var fieldOffset = aggregateOffset.Offset;
        var fieldSize = (Bits) (uint) (nextOffset?.AggregateSize ?? (uint) value.Size);
        var fieldBuffer = Values.Read.Create(collectionFactory, solver, buffer, fieldOffset, fieldSize);

        return nextOffset is null
            ? CreateLeaf(collectionFactory, solver, fieldBuffer, fieldOffset, value)
            : new AggregateWrite(
                fieldBuffer,
                fieldOffset,
                ImmutableList.Create(CreateField(collectionFactory, solver, fieldBuffer, nextOffset, value)));
    }

    private static AggregateWrite CreateLeaf(ICollectionFactory collectionFactory, ISolver solver,
        IValue buffer, IValue offset, IValue value)
    {
        return new AggregateWrite(
            value.Size == buffer.Size
                ? value
                : Values.Write.Create(collectionFactory, solver, buffer, offset, value),
            offset,
            ImmutableList.Create<AggregateWrite>());
    }
}
