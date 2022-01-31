using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class Write : BitVector, IWriteValue
{
    private readonly IValue _writeBuffer;
    private readonly IValue _writeOffset;
    private readonly IValue _writeValue;

    public Write(IValue writeBuffer, IValue writeOffset, IValue writeValue)
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

    public IValue Read(IValue offset, Bits size)
    {
        var readMask = Mask(offset, size);
        var writeMask = Mask(_writeOffset, _writeValue.Size);

        return readMask is IConstantValue r && writeMask is IConstantValue w
            ? r.AsUnsigned().And(w.AsUnsigned()).IsZero
                ? _writeBuffer is IWriteValue b
                    ? b.Read(offset, size)
                    : new Read(_writeBuffer, offset, size)
                : r.AsUnsigned().Xor(w.AsUnsigned()).IsZero
                    ? _writeValue
                    : new Read(Flatten(), offset, size)
            : new Read(Flatten(), offset, size);
    }

    private IValue Flatten()
    {
        var writeMask = Mask(_writeOffset, _writeValue.Size);
        var writeData = ShiftLeft(ZeroExtend(Size, _writeValue), _writeOffset);

        return Or(And(_writeBuffer, Not(writeMask)), writeData);
    }

    private IValue Mask(IValue offset, Bits size)
    {
        return ShiftLeft(ZeroExtend(Size, Not(ConstantUnsigned.Create(size, BigInteger.Zero))), offset);
    }

    private static IValue And(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().And(r.AsUnsigned())
            : new And(left, right);
    }

    private static IValue Not(IValue value)
    {
        return value is IConstantValue v
            ? v.AsUnsigned().Not()
            : new Not(value);
    }

    private static IValue Or(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().Or(r.AsUnsigned())
            : new Or(left, right);
    }

    private static IValue ShiftLeft(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().ShiftLeft(r.AsUnsigned())
            : new ShiftLeft(left, right);
    }

    private static IValue ZeroExtend(Bits size, IValue value)
    {
        return value is IConstantValue v
            ? v.AsUnsigned().Extend(size)
            : new ZeroExtend(size, value);
    }
}
