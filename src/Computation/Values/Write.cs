using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record Write : BitVector
{
    private readonly IValue _writeBuffer;
    private readonly IValue _writeMask;
    private readonly IValue _writeOffset;
    private readonly IValue _writeValue;

    private Write(IValue writeBuffer, IValue writeOffset, IValue writeValue)
        : base(writeBuffer.Size)
    {
        _writeBuffer = writeBuffer;
        _writeOffset = writeOffset;
        _writeValue = writeValue;
        _writeMask = Mask(writeBuffer, writeOffset, writeValue.Size);
    }

    public override IEnumerable<IValue> Children => new[] { _writeBuffer, _writeOffset, _writeValue };

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        return Flatten().AsBitVector(solver);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as Write);
    }

    public IValue LayerRead(ICollectionFactory collectionFactory, ISolver solver, IValue offset, Bits size)
    {
        var mask = Mask(this, offset, size);

        return IsNotOverlapping(solver, mask)
            ? Read.Create(collectionFactory, solver, _writeBuffer, offset, size)
            : IsAligned(solver, mask)
                ? _writeValue
                : Read.Create(collectionFactory, solver, Flatten(), offset, size);
    }

    public IValue ReadAggregate(ICollectionFactory collectionFactory, ISolver solver, AggregateOffset aggregateOffset, Bits size)
    {
        var mask = Mask(this, aggregateOffset.BaseAddress, aggregateOffset.Size);

        return IsNotOverlapping(solver, mask)
            ? Read.Create(collectionFactory, solver, _writeBuffer, aggregateOffset, size)
            : IsAligned(solver, mask)
                ? Read.Create(collectionFactory, solver, _writeValue, aggregateOffset, size)
                : Read.Create(collectionFactory, solver, Flatten(), aggregateOffset, size);
    }

    private IValue LayerWrite(ICollectionFactory collectionFactory, ISolver solver, IValue offset, IValue value)
    {
        var mask = Mask(this, offset, value.Size);

        return IsNotOverlapping(solver, mask)
            ? new Write(Create(collectionFactory, solver, _writeBuffer, offset, value), _writeOffset, _writeValue)
            : IsAligned(solver, mask)
                ? new Write(_writeBuffer, offset, value)
                : new Write(this, offset, value);
    }

    private Write WriteAggregate(ICollectionFactory collectionFactory, ISolver solver, AggregateOffset aggregateOffset, IValue value)
    {
        var mask = Mask(this, aggregateOffset.BaseAddress, aggregateOffset.Size);

        return IsNotOverlapping(solver, mask)
            ? new Write(Create(collectionFactory, solver, _writeBuffer, aggregateOffset, value), _writeOffset, _writeValue)
            : IsAligned(solver, mask)
                ? _writeValue is AggregateWrite aw
                    ? new Write(_writeBuffer, aggregateOffset.BaseAddress, aw.Write(collectionFactory, solver, aggregateOffset, value))
                    : new Write(_writeBuffer, aggregateOffset.BaseAddress, AggregateWrite.Create(collectionFactory, solver, _writeBuffer, aggregateOffset, value))
                : new Write(this, aggregateOffset.BaseAddress, AggregateWrite.Create(collectionFactory, solver, this, aggregateOffset, value));
    }

    private bool IsNotOverlapping(ISolver solver, IValue mask)
    {
        var isOverlapping = And.Create(mask, _writeMask);
        return !solver.IsSatisfiable(isOverlapping);
    }

    private bool IsAligned(ISolver solver, IValue mask)
    {
        var isNotAligned = Xor.Create(mask, _writeMask);
        return !solver.IsSatisfiable(isNotAligned);
    }

    private IValue Flatten()
    {
        var writeData = ShiftLeft.Create(ZeroExtend.Create(Size, _writeValue), ZeroExtend.Create(Size, _writeOffset));

        return Or.Create(And.Create(_writeBuffer, Not.Create(_writeMask)), writeData);
    }

    private static IValue Mask(IValue buffer, IValue offset, Bits size)
    {
        return ShiftLeft.Create(ConstantUnsigned.CreateZero(size).Not().Extend(buffer.Size), ZeroExtend.Create(buffer.Size, offset));
    }

    public static IValue Create(ICollectionFactory collectionFactory, ISolver solver,
        IValue buffer, IValue offset, IValue value)
    {
        // TODO: Split out indices from AggregateOffset so that we can distinguish between
        // the entry point write and recursive ones.
        // Then we can more easily merge these cases together because the initial case can do
        // Create(collectionFactory, assertions, buffer, ao.BaseAddress, AggregateWrite.Create(buffer, ao, value))
        if (offset is AggregateOffset ao && ao.IsBounded(solver, value.Size))
        {
            return buffer is AggregateWrite aw
                ? aw.Write(collectionFactory, solver, ao, value)
                : buffer is Write w1
                    ? w1.WriteAggregate(collectionFactory, solver, ao, value)
                    : new Write(buffer, ao.BaseAddress, AggregateWrite.Create(collectionFactory, solver, buffer, ao, value));
        }
        return buffer is IConstantValue b && offset is IConstantValue o && value is IConstantValue v
            ? b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory))
            : buffer is Write w
                ? w.LayerWrite(collectionFactory, solver, offset, value)
                : new Write(buffer, offset, value);
    }
}
