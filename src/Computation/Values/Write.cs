using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class Write : BitVector
{
    public enum Overlapping
    {
        Unknown,
        None,
        Some
    }

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

    private IValue CumulativeMask => _writeBuffer is Write w ? Or.Create(_writeMask, w.CumulativeMask) : _writeMask;

    public override BitVecExpr AsBitVector(IContext context)
    {
        return Flatten().AsBitVector(context);
    }

    public IValue LayerRead(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue offset, Bits size, Overlapping overlapping)
    {
        var mask = Mask(this, offset, size);

        if (mask is not IConstantValue && (overlapping == Overlapping.None || (overlapping == Overlapping.Unknown && IsNotOverlappingAnyLayer(assertions, mask))))
        {
            Debug.Assert(mask is not IConstantValue);
            return Read.Create(collectionFactory, assertions, _writeBuffer, offset, size, Overlapping.None);
        }

        return IsNotOverlapping(assertions, mask)
            ? Read.Create(collectionFactory, assertions, _writeBuffer, offset, size, Overlapping.Some)
            : IsAligned(assertions, mask)
                ? _writeValue
                : Read.Create(collectionFactory, assertions, Flatten(), offset, size, Overlapping.Some);
    }

    private IValue LayerWrite(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue offset, IValue value, Overlapping overlapping)
    {
        var mask = Mask(this, offset, value.Size);

        if (mask is not IConstantValue && (overlapping == Overlapping.None || (overlapping == Overlapping.Unknown && IsNotOverlappingAnyLayer(assertions, mask))))
        {
            Debug.Assert(mask is not IConstantValue);
            return new Write(Create(collectionFactory, assertions, _writeBuffer, offset, value, Overlapping.None), _writeOffset, _writeValue);
        }

        return IsNotOverlapping(assertions, mask)
            ? new Write(Create(collectionFactory, assertions, _writeBuffer, offset, value, Overlapping.Some), _writeOffset, _writeValue)
            : IsAligned(assertions, mask)
                ? new Write(_writeBuffer, offset, value)
                : new Write(this, offset, value);
    }

    private bool IsNotOverlappingAnyLayer(IAssertions assertions, IValue mask)
    {
        var isOverlapping = And.Create(mask, CumulativeMask);
        using var proposition = assertions.GetProposition(isOverlapping);

        return !proposition.CanBeTrue;
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
        var writeData = ShiftLeft.Create(ZeroExtend.Create(Size, _writeValue), _writeOffset);

        return Or.Create(And.Create(_writeBuffer, Not.Create(_writeMask)), writeData);
    }

    private static IValue Mask(IValue buffer, IValue offset, Bits size)
    {
        return ShiftLeft.Create(ConstantUnsigned.Create(size, BigInteger.Zero).Not().Extend(buffer.Size), offset);
    }

    public static IValue Create(ICollectionFactory collectionFactory, IAssertions assertions,
        IValue buffer, IValue offset, IValue value, Overlapping overlapping = Overlapping.Unknown)
    {
        return buffer is IConstantValue b && offset is IConstantValue o && value is IConstantValue v
            ? b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory))
            : buffer is Write w
                ? w.LayerWrite(collectionFactory, assertions, offset, value, overlapping)
                : new Write(buffer, offset, value);
    }
}
