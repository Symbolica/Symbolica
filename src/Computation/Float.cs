using System;
using System.Collections.Generic;
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
    public abstract IEnumerable<IValue> Children { get; }
    public abstract string? PrintedValue { get; }

    public BitVecExpr AsBitVector(IContext context)
    {
        using var flt = AsFloat(context);
        return context.CreateExpr(c => c.MkFPToIEEEBV(flt));
    }

    public BoolExpr AsBool(IContext context)
    {
        using var flt = AsFloat(context);
        using var isZero = context.CreateExpr(c => c.MkFPIsZero(flt));
        return context.CreateExpr(c => c.MkNot(isZero));
    }

    public abstract FPExpr AsFloat(IContext context);

    public IValue BitCast(Bits targetSize) => this;

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
