using System;
using Microsoft.Z3;
using Symbolica.Computation.Values.Symbolics;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal abstract class Value : IValue
{
    public abstract Bits Size { get; }

    public abstract BitVecExpr AsBitVector(Context context);
    public abstract BoolExpr AsBool(Context context);
    public abstract FPExpr AsFloat(Context context);

    public static IValue Select(IValue predicate, IValue trueValue, IValue falseValue)
    {
        return Ternary(predicate, trueValue, falseValue,
            (p, t, f) => p.AsBool() ? t : f,
            (p, t, f) => new Select(p, t, f));
    }

    protected static IValue Unary(IValue x,
        Func<IConstantValue, IValue> constant,
        Func<IValue, IValue> symbolic)
    {
        return x is IConstantValue cx
            ? constant(cx)
            : symbolic(x);
    }

    protected static IValue Binary(IValue x, IValue y,
        Func<IConstantValue, IConstantValue, IValue> constant,
        Func<IValue, IValue, IValue> symbolic)
    {
        return x is IConstantValue cx && y is IConstantValue cy
            ? constant(cx, cy)
            : symbolic(x, y);
    }

    protected static IValue Ternary(IValue x, IValue y, IValue z,
        Func<IConstantValue, IConstantValue, IConstantValue, IValue> constant,
        Func<IValue, IValue, IValue, IValue> symbolic)
    {
        return x is IConstantValue cx && y is IConstantValue cy && z is IConstantValue cz
            ? constant(cx, cy, cz)
            : symbolic(x, y, z);
    }
}
