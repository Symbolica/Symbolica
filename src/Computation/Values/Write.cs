using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class Write : BitVector
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

    public override BitVecExpr AsBitVector(IContext context)
    {
        return Flatten().AsBitVector(context);
    }

    public IValue LayerRead(ICollectionFactory collectionFactory, IAssertions assertions, IValue offset, Bits size)
    {
        var mask = Mask(this, offset, size);

        return IsNotOverlapping(assertions, mask)
            ? Read.Create(collectionFactory, assertions, _writeBuffer, offset, size)
            : IsAligned(assertions, mask)
                ? _writeValue
                : Read.Create(collectionFactory, assertions, Flatten(), offset, size);
    }

    public IValue ReadAggregate(ICollectionFactory collectionFactory, IAssertions assertions, AggregateOffset aggregateOffset, Bits size)
    {
        var mask = Mask(this, aggregateOffset.BaseAddress, aggregateOffset.Size);

        return IsNotOverlapping(assertions, mask)
            ? Read.Create(collectionFactory, assertions, _writeBuffer, aggregateOffset, size)
            : IsAligned(assertions, mask)
                ? Read.Create(collectionFactory, assertions, _writeValue, aggregateOffset, size)
                : Read.Create(collectionFactory, assertions, Flatten(), aggregateOffset, size);
    }

    private IValue LayerWrite(ICollectionFactory collectionFactory, IAssertions assertions, IValue offset, IValue value)
    {
        var mask = Mask(this, offset, value.Size);

        return IsNotOverlapping(assertions, mask)
            ? new Write(Create(collectionFactory, assertions, _writeBuffer, offset, value), _writeOffset, _writeValue)
            : IsAligned(assertions, mask)
                ? new Write(_writeBuffer, offset, value)
                : new Write(this, offset, value);
    }

    private Write WriteAggregate(ICollectionFactory collectionFactory, IAssertions assertions, AggregateOffset aggregateOffset, IValue value)
    {
        var mask = Mask(this, aggregateOffset.BaseAddress, aggregateOffset.Size);

        return IsNotOverlapping(assertions, mask)
            ? new Write(Create(collectionFactory, assertions, _writeBuffer, aggregateOffset, value), _writeOffset, _writeValue)
            : IsAligned(assertions, mask)
                ? _writeValue is AggregateWrite aw
                    ? new Write(_writeBuffer, aggregateOffset.BaseAddress, aw.Write(collectionFactory, assertions, aggregateOffset, value))
                    : new Write(_writeBuffer, aggregateOffset.BaseAddress, AggregateWrite.Create(collectionFactory, assertions, _writeBuffer, aggregateOffset, value))
                : new Write(this, aggregateOffset.BaseAddress, AggregateWrite.Create(collectionFactory, assertions, this, aggregateOffset, value));
    }

    private bool IsNotOverlapping(IAssertions assertions, IValue mask)
    {
        var isOverlapping = And.Create(mask, _writeMask);
        using var proposition = assertions.GetProposition(isOverlapping);

        return !proposition.CanBeTrue;
    }

    private bool IsAligned(IAssertions assertions, IValue mask)
    {
        var isNotAligned = Xor.Create(mask, _writeMask);
        using var proposition = assertions.GetProposition(isNotAligned);

        return !proposition.CanBeTrue;
    }

    private IValue Flatten()
    {
        var writeData = ShiftLeft.Create(ZeroExtend.Create(Size, _writeValue), ZeroExtend.Create(Size, _writeOffset));

        return Or.Create(And.Create(_writeBuffer, Not.Create(_writeMask)), writeData);
    }

    private static IValue Mask(IValue buffer, IValue offset, Bits size)
    {
        return ShiftLeft.Create(ConstantUnsigned.Create(size, BigInteger.Zero).Not().Extend(buffer.Size), ZeroExtend.Create(buffer.Size, offset));
    }

    public static IValue Create(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue buffer, IValue offset, IValue value)
    {
        // TODO: Split out indices from AggregateOffset so that we can distinguish between
        // the entry point write and recursive ones.
        // Then we can more easily merge these cases together because the initial case can do
        // Create(collectionFactory, assertions, buffer, ao.BaseAddress, AggregateWrite.Create(buffer, ao, value))
        if (offset is AggregateOffset ao && ao.IsBounded(assertions, value.Size))
        {
            return buffer is AggregateWrite aw
                ? aw.Write(collectionFactory, assertions, ao, value)
                : buffer is Write w1
                    ? w1.WriteAggregate(collectionFactory, assertions, ao, value)
                    : new Write(buffer, ao.BaseAddress, AggregateWrite.Create(collectionFactory, assertions, buffer, ao, value));
        }
        return buffer is IConstantValue b && offset is IConstantValue o && value is IConstantValue v
            ? b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory))
            : buffer is Write w
                ? w.LayerWrite(collectionFactory, assertions, offset, value)
                : new Write(buffer, offset, value);
    }
}
