using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Computation.Values.Symbolics;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal abstract class Float : Value
{
    protected Float(Bits size)
    {
        Size = size;
    }

    public override Bits Size { get; }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkFPToIEEEBV(AsFloat(context));
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkNot(context.MkFPIsZero(AsFloat(context)));
    }

    public static IValue Add(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantSingle(l + r),
            (l, r) => new ConstantDouble(l + r),
            (l, r) => new FloatAdd(l, r));
    }

    public static IValue Ceiling(IValue value)
    {
        return Unary(value,
            v => new ConstantSingle(MathF.Ceiling(v)),
            v => new ConstantDouble(Math.Ceiling(v)),
            v => new FloatCeiling(v));
    }

    public static IValue Convert(Bits size, IValue value)
    {
        return Unary(value,
            v => (uint) size switch
            {
                32U => new ConstantSingle(v),
                64U => new ConstantDouble(v),
                _ => new FloatConvert(size, new ConstantSingle(v))
            },
            v => (uint) size switch
            {
                32U => new ConstantSingle((float) v),
                64U => new ConstantDouble(v),
                _ => new FloatConvert(size, new ConstantDouble(v))
            },
            v => v is IRealValue r
                ? new RealConvert(size, r)
                : new FloatConvert(size, v));
    }

    public static IValue Divide(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantSingle(l / r),
            (l, r) => new ConstantDouble(l / r),
            (l, r) => new FloatDivide(l, r));
    }

    public static IValue Equal(IValue left, IValue right)
    {
        return Binary(left, right,
            // ReSharper disable CompareOfFloatsByEqualityOperator
            (l, r) => new ConstantBool(l == r),
            (l, r) => new ConstantBool(l == r),
            // ReSharper restore CompareOfFloatsByEqualityOperator
            (l, r) => new FloatEqual(l, r));
    }

    public static IValue Floor(IValue value)
    {
        return Unary(value,
            v => new ConstantSingle(MathF.Floor(v)),
            v => new ConstantDouble(Math.Floor(v)),
            v => new FloatFloor(v));
    }

    public static IValue Greater(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantBool(l > r),
            (l, r) => new ConstantBool(l > r),
            (l, r) => new FloatGreater(l, r));
    }

    public static IValue GreaterOrEqual(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantBool(l >= r),
            (l, r) => new ConstantBool(l >= r),
            (l, r) => new FloatGreaterOrEqual(l, r));
    }

    public static IValue Less(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantBool(l < r),
            (l, r) => new ConstantBool(l < r),
            (l, r) => new FloatLess(l, r));
    }

    public static IValue LessOrEqual(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantBool(l <= r),
            (l, r) => new ConstantBool(l <= r),
            (l, r) => new FloatLessOrEqual(l, r));
    }

    public static IValue Multiply(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantSingle(l * r),
            (l, r) => new ConstantDouble(l * r),
            (l, r) => new FloatMultiply(l, r));
    }

    public static IValue Negate(IValue value)
    {
        return Unary(value,
            v => new ConstantSingle(-v),
            v => new ConstantDouble(-v),
            v => new FloatNegate(v));
    }

    public static IValue NotEqual(IValue left, IValue right)
    {
        return Binary(left, right,
            // ReSharper disable CompareOfFloatsByEqualityOperator
            (l, r) => new ConstantBool(l != r),
            (l, r) => new ConstantBool(l != r),
            // ReSharper restore CompareOfFloatsByEqualityOperator
            (l, r) => new Not(new FloatEqual(l, r)));
    }

    public static IValue Ordered(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantBool(!(float.IsNaN(l) || float.IsNaN(r))),
            (l, r) => new ConstantBool(!(double.IsNaN(l) || double.IsNaN(r))),
            (l, r) => new Not(new FloatUnordered(l, r)));
    }

    public static IValue Power(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantSingle(MathF.Pow(l, r)),
            (l, r) => new ConstantDouble(Math.Pow(l, r)),
            (l, r) => new FloatPower(l, r));
    }

    public static IValue Remainder(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantSingle(MathF.IEEERemainder(l, r)),
            (l, r) => new ConstantDouble(Math.IEEERemainder(l, r)),
            (l, r) => new FloatRemainder(l, r));
    }

    public static IValue Subtract(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantSingle(l - r),
            (l, r) => new ConstantDouble(l - r),
            (l, r) => new FloatSubtract(l, r));
    }

    public static IValue ToSigned(Bits size, IValue value)
    {
        return Unary(value,
            v => ConstantSigned.Create(size, (BigInteger) v),
            v => ConstantSigned.Create(size, (BigInteger) v),
            v => v is IRealValue r
                ? new RealToSigned(size, r)
                : new FloatToSigned(size, v));
    }

    public static IValue ToUnsigned(Bits size, IValue value)
    {
        return Unary(value,
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            v => new FloatToUnsigned(size, v));
    }

    public static IValue Unordered(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantBool(float.IsNaN(l) || float.IsNaN(r)),
            (l, r) => new ConstantBool(double.IsNaN(l) || double.IsNaN(r)),
            (l, r) => new FloatUnordered(l, r));
    }

    private static IValue Unary(IValue x,
        Func<float, IValue> constantSingle,
        Func<double, IValue> constantDouble,
        Func<IValue, IValue> symbolic)
    {
        return Unary(x,
            a => (uint) a.Size switch
            {
                32U => constantSingle(a.AsSingle()),
                64U => constantDouble(a.AsDouble()),
                _ => symbolic(a)
            },
            symbolic);
    }

    private static IValue Binary(IValue x, IValue y,
        Func<float, float, IValue> constantSingle,
        Func<double, double, IValue> constantDouble,
        Func<IValue, IValue, IValue> symbolic)
    {
        return Binary(x, y,
            (a, b) => (uint) a.Size switch
            {
                32U => constantSingle(a.AsSingle(), b.AsSingle()),
                64U => constantDouble(a.AsDouble(), b.AsDouble()),
                _ => symbolic(a, b)
            },
            symbolic);
    }
}
