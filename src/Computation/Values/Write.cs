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

    private Write(IValue writeBuffer, IValue writeOffset, IValue writeValue)
        : base(writeBuffer.Size)
    {
        _writeBuffer = writeBuffer;
        _writeOffset = writeOffset;
        _writeValue = writeValue;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return Flatten().AsBitVector(context);
    }

    public IValue Read(ICollectionFactory collectionFactory, IValue offset, Bits size)
    {
        var readMask = Mask(offset, size);
        var writeMask = Mask(_writeOffset, _writeValue.Size);

        return And.Create(readMask, writeMask) is IConstantValue a && a.AsUnsigned().IsZero
            ? _writeBuffer is Write w
                ? w.Read(collectionFactory, offset, size)
                : Values.Read.Create(collectionFactory, _writeBuffer, offset, size)
            : Xor.Create(readMask, writeMask) is IConstantValue x && x.AsUnsigned().IsZero
                ? _writeValue
                : Values.Read.Create(collectionFactory, Flatten(), offset, size);
    }

    private IValue Flatten()
    {
        var writeMask = Mask(_writeOffset, _writeValue.Size);
        var writeData = ShiftLeft.Create(ZeroExtend.Create(Size, _writeValue), _writeOffset);

        return Or.Create(And.Create(_writeBuffer, Not.Create(writeMask)), writeData);
    }

    private IValue Mask(IValue offset, Bits size)
    {
        return ShiftLeft.Create(ConstantUnsigned.Create(size, BigInteger.Zero).Not().Extend(Size), offset);
    }

    public static IValue Create(ICollectionFactory collectionFactory, IValue buffer, IValue offset, IValue value)
    {
        return Value.Create(buffer, offset, value,
            (b, o, v) => b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory)),
            (b, o, v) => new Write(b, o, v));
    }
}
