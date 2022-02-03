using System.Numerics;
using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class Write : BitVector
{
    private readonly IValue _writeBuffer;
    private readonly IValue _writeOffset;
    private readonly IValue _writeValue;
    private readonly IValue _writeMask;

    private Write(IValue writeBuffer, IValue writeOffset, IValue writeValue)
        : base(writeBuffer.Size)
    {
        _writeBuffer = writeBuffer;
        _writeOffset = writeOffset;
        _writeValue = writeValue;
        _writeMask = Mask(writeBuffer, writeOffset, writeValue.Size);
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return Flatten().AsBitVector(context);
    }

    public IValue Read(ICollectionFactory collectionFactory, IValue offset, Bits size)
    {
        var readMask = Mask(this, offset, size);

        return NotOverlapsWith(readMask)
            ? _writeBuffer is Write w
                ? w.Read(collectionFactory, offset, size)
                : Values.Read.Create(collectionFactory, _writeBuffer, offset, size)
            : ExactlyAlignsWith(readMask)
                ? _writeValue
                : Values.Read.Create(collectionFactory, Flatten(), offset, size);
    }

    private IValue Flatten()
    {
        var writeData = ShiftLeft.Create(ZeroExtend.Create(Size, _writeValue), _writeOffset);

        return Or.Create(And.Create(_writeBuffer, Not.Create(_writeMask)), writeData);
    }

    private bool ExactlyAlignsWith(IValue mask)
    {
        return Xor.Create(mask, _writeMask) is IConstantValue x && x.AsUnsigned().IsZero;
    }

    private bool NotOverlapsWith(IValue mask)
    {
        return And.Create(mask, _writeMask) is IConstantValue a && a.AsUnsigned().IsZero;
    }

    private static IValue Mask(IValue buffer, IValue offset, Bits size)
    {
        return ShiftLeft.Create(ConstantUnsigned.Create(size, BigInteger.Zero).Not().Extend(buffer.Size), offset);
    }

    public static IValue Create(ICollectionFactory collectionFactory, IValue buffer, IValue offset, IValue value)
    {
        return Value.Create(buffer, offset, value,
            (b, o, v) => b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory)),
            (b, o, v) =>
            {
                var mask = Mask(b, o, v.Size);
                return b is Write w
                    ? w.NotOverlapsWith(mask)
                        ? new Write(Create(collectionFactory, w._writeBuffer, o, v), w._writeOffset, w._writeValue)
                        : w.ExactlyAlignsWith(mask)
                            ? new Write(w._writeBuffer, o, v)
                            : new Write(b, o, v)
                    : new Write(b, o, v);
            });
    }
}
