using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class Write : BitVector
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

        return And(readMask, writeMask) is IConstantValue a && a.AsUnsigned().IsZero
            ? _writeBuffer is Write b
                ? b.Read(offset, size)
                : Read(_writeBuffer, offset, size)
            : Xor(readMask, writeMask) is IConstantValue x && x.AsUnsigned().IsZero
                ? _writeValue
                : Read(Flatten(), offset, size);
    }

    private static IValue Read(IValue buffer, IValue offset, Bits size)
    {
        return Truncate(size, LogicalShiftRight(buffer, offset));
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

    private static IValue LogicalShiftRight(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().ShiftRight(r.AsUnsigned())
            : new LogicalShiftRight(left, right);
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

    private static IValue Truncate(Bits size, IValue value)
    {
        return value is IConstantValue v
            ? v.AsUnsigned().Truncate(size)
            : new Truncate(size, value);
    }

    private static IValue Xor(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().Xor(r.AsUnsigned())
            : new Xor(left, right);
    }

    private static IValue ZeroExtend(Bits size, IValue value)
    {
        return value is IConstantValue v
            ? v.AsUnsigned().Extend(size)
            : new ZeroExtend(size, value);
    }
}
