using System;
using System.Numerics;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract record Float : IValue
{
    protected Float(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }

    public BitVecExpr AsBitVector(ISolver solver)
    {
        using var flt = AsFloat(solver);
        using var simplified = flt.Simplify();

        return simplified.IsFPNaN
            ? CreateNan(solver)
            : solver.Context.MkFPToIEEEBV(flt);
    }

    public BoolExpr AsBool(ISolver solver)
    {
        using var flt = AsFloat(solver);
        using var isZero = solver.Context.MkFPIsZero(flt);
        return solver.Context.MkNot(isZero);
    }

    public abstract FPExpr AsFloat(ISolver solver);
    public abstract bool Equals(IValue? other);

    private BitVecExpr CreateNan(ISolver solver)
    {
        using var sort = Size.GetSort(solver);
        var nan = ((BigInteger.One << ((int) sort.EBits + 2)) - BigInteger.One) << ((int) sort.SBits - 2);

        return solver.Context.MkBV(nan.ToString(), (uint) Size);
    }

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
