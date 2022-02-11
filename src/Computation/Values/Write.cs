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

    public override BitVecExpr AsBitVector(IContext context)
    {
        return Flatten().AsBitVector(context);
    }

    public IValue LayerRead(ICollectionFactory collectionFactory, IValue offset, Bits size)
    {
        var mask = Mask(this, offset, size);

        return IsNotOverlapping(mask)
            ? Read.Create(collectionFactory, _writeBuffer, offset, size)
            : IsAligned(mask)
                ? _writeValue
                : Read.Create(collectionFactory, Flatten(), offset, size);
    }

    private IValue LayerWrite(ICollectionFactory collectionFactory, IValue offset, IValue value)
    {
        var mask = Mask(this, offset, value.Size);

        return IsNotOverlapping(mask)
            ? new Write(Create(collectionFactory, _writeBuffer, offset, value), _writeOffset, _writeValue)
            : IsAligned(mask)
                ? new Write(_writeBuffer, offset, value)
                : new Write(this, offset, value);
    }

    private bool IsNotOverlapping(IValue mask)
    {
        return And.Create(mask, _writeMask) is IConstantValue a && a.AsUnsigned().IsZero;
    }

    private bool IsAligned(IValue mask)
    {
        return Xor.Create(mask, _writeMask) is IConstantValue x && x.AsUnsigned().IsZero;
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

    public static IValue Create(ICollectionFactory collectionFactory, IValue buffer, IValue offset, IValue value)
    {
        return buffer is IConstantValue b && offset is IConstantValue o && value is IConstantValue v
            ? b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory))
            : buffer is Write w
                ? w.LayerWrite(collectionFactory, offset, value)
                : new Write(buffer, offset, value);
    }
}
