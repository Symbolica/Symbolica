using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract class Float : IValue
{
    protected Float(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }

    public BigInteger AsConstant(Context context)
    {
        return AsFloat(context).Simplify().IsFPNaN
            ? Size.GetNan(context)
            : AsBitVector(context).AsConstant();
    }

    public BitVecExpr AsBitVector(Context context)
    {
        return context.MkFPToIEEEBV(AsFloat(context));
    }

    public BoolExpr AsBool(Context context)
    {
        return context.MkNot(context.MkFPIsZero(AsFloat(context)));
    }

    public abstract FPExpr AsFloat(Context context);

    public static IValue Unary(IValue value,
        Func<float, IValue> constantSingle,
        Func<double, IValue> constantDouble,
        Func<IValue, IValue> symbolic)
    {
        return value is IConstantValue x
            ? (uint) x.Size switch
            {
                32U => constantSingle(x.AsSingle()),
                64U => constantDouble(x.AsDouble()),
                _ => symbolic(x)
            }
            : symbolic(value);
    }

    public static IValue Binary(IValue left, IValue right,
        Func<float, float, IValue> constantSingle,
        Func<double, double, IValue> constantDouble,
        Func<IValue, IValue, IValue> symbolic)
    {
        return left is IConstantValue x && right is IConstantValue y
            ? (uint) x.Size switch
            {
                32U => constantSingle(x.AsSingle(), y.AsSingle()),
                64U => constantDouble(x.AsDouble(), y.AsDouble()),
                _ => symbolic(x, y)
            }
            : symbolic(left, right);
    }
}
