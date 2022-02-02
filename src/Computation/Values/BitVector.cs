using Microsoft.Z3;
using Symbolica.Collection;
using Symbolica.Computation.Values.Symbolics;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal abstract class BitVector : Integer
{
    protected BitVector(Bits size)
        : base(size)
    {
    }

    public sealed override BoolExpr AsBool(Context context)
    {
        return context.MkNot(context.MkEq(AsBitVector(context), context.MkBV(0U, (uint) Size)));
    }

    public static IValue Add(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().Add(r.AsUnsigned()),
            (l, r) => new Add(l, r));
    }

    public static IValue ArithmeticShiftRight(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsSigned().ShiftRight(r.AsUnsigned()),
            (l, r) => new ArithmeticShiftRight(l, r));
    }

    public static IValue Equal(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().Equal(r.AsUnsigned()),
            (l, r) => new Equal(l, r));
    }

    public static IValue LogicalShiftRight(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().ShiftRight(r.AsUnsigned()),
            (l, r) => new LogicalShiftRight(l, r));
    }

    public static IValue Multiply(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().Multiply(r.AsUnsigned()),
            (l, r) => new Multiply(l, r));
    }

    public static IValue NotEqual(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().NotEqual(r.AsUnsigned()),
            (l, r) => new Not(new Equal(l, r)));
    }

    public static IValue Read(ICollectionFactory collectionFactory, IValue buffer, IValue offset, Bits size)
    {
        return Binary(buffer, offset,
            (b, o) => b.AsBitVector(collectionFactory).Read(o.AsUnsigned(), size),
            (b, o) => b is Write w
                ? w.Read(o, size)
                : new Truncate(size, new LogicalShiftRight(b, o)));
    }

    public static IValue ShiftLeft(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().ShiftLeft(r.AsUnsigned()),
            (l, r) => new ShiftLeft(l, r));
    }

    public static IValue SignedDivide(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsSigned().Divide(r.AsSigned()),
            (l, r) => new SignedDivide(l, r));
    }

    public static IValue SignedGreater(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsSigned().Greater(r.AsSigned()),
            (l, r) => new SignedGreater(l, r));
    }

    public static IValue SignedGreaterOrEqual(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsSigned().GreaterOrEqual(r.AsSigned()),
            (l, r) => new SignedGreaterOrEqual(l, r));
    }

    public static IValue SignedLess(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsSigned().Less(r.AsSigned()),
            (l, r) => new SignedLess(l, r));
    }

    public static IValue SignedLessOrEqual(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsSigned().LessOrEqual(r.AsSigned()),
            (l, r) => new SignedLessOrEqual(l, r));
    }

    public static IValue SignedRemainder(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsSigned().Remainder(r.AsSigned()),
            (l, r) => new SignedRemainder(l, r));
    }

    public static IValue SignedToFloat(Bits size, IValue value)
    {
        return Unary(value,
            v => (uint) size switch
            {
                32U => v.AsSigned().ToSingle(),
                64U => v.AsSigned().ToDouble(),
                _ => new SignedToFloat(size, v)
            },
            v => new SignedToFloat(size, v));
    }

    public static IValue SignExtend(Bits size, IValue value)
    {
        return Unary(value,
            v => v.AsSigned().Extend(size),
            v => new SignExtend(size, v));
    }

    public static IValue Subtract(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().Subtract(r.AsUnsigned()),
            (l, r) => new Subtract(l, r));
    }

    public static IValue Truncate(Bits size, IValue value)
    {
        return Unary(value,
            v => v.AsUnsigned().Truncate(size),
            v => new Truncate(size, v));
    }

    public static IValue UnsignedDivide(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().Divide(r.AsUnsigned()),
            (l, r) => new UnsignedDivide(l, r));
    }

    public static IValue UnsignedGreater(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().Greater(r.AsUnsigned()),
            (l, r) => new UnsignedGreater(l, r));
    }

    public static IValue UnsignedGreaterOrEqual(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().GreaterOrEqual(r.AsUnsigned()),
            (l, r) => new UnsignedGreaterOrEqual(l, r));
    }

    public static IValue UnsignedLess(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().Less(r.AsUnsigned()),
            (l, r) => new UnsignedLess(l, r));
    }

    public static IValue UnsignedLessOrEqual(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().LessOrEqual(r.AsUnsigned()),
            (l, r) => new UnsignedLessOrEqual(l, r));
    }

    public static IValue UnsignedRemainder(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => l.AsUnsigned().Remainder(r.AsUnsigned()),
            (l, r) => new UnsignedRemainder(l, r));
    }

    public static IValue UnsignedToFloat(Bits size, IValue value)
    {
        return Unary(value,
            v => (uint) size switch
            {
                32U => v.AsUnsigned().ToSingle(),
                64U => v.AsUnsigned().ToDouble(),
                _ => new UnsignedToFloat(size, v)
            },
            v => new UnsignedToFloat(size, v));
    }

    public static IValue Write(ICollectionFactory collectionFactory, IValue buffer, IValue offset, IValue value)
    {
        return Ternary(buffer, offset, value,
            (b, o, v) => b.AsBitVector(collectionFactory).Write(o.AsUnsigned(), v.AsBitVector(collectionFactory)),
            (b, o, v) => new Write(b, o, v));
    }

    public static IValue ZeroExtend(Bits size, IValue value)
    {
        return Unary(value,
            v => v.AsUnsigned().Extend(size),
            v => new ZeroExtend(size, v));
    }
}
