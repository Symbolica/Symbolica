using System;

namespace Symbolica.Computation.Values;

internal static class Value
{
    public static IValue Unary(IValue x,
        Func<IConstantValue, IValue> constant,
        Func<IValue, IValue> symbolic)
    {
        return x is IConstantValue cx
            ? constant(cx)
            : symbolic(x);
    }

    public static IValue Binary(IValue x, IValue y,
        Func<IConstantValue, IConstantValue, IValue> constant,
        Func<IValue, IValue, IValue> symbolic)
    {
        return x is IConstantValue cx && y is IConstantValue cy
            ? constant(cx, cy)
            : symbolic(x, y);
    }

    public static IValue Ternary(IValue x, IValue y, IValue z,
        Func<IConstantValue, IConstantValue, IConstantValue, IValue> constant,
        Func<IValue, IValue, IValue, IValue> symbolic)
    {
        return x is IConstantValue cx && y is IConstantValue cy && z is IConstantValue cz
            ? constant(cx, cy, cz)
            : symbolic(x, y, z);
    }

    public static IValue Unary(IValue x,
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

    public static IValue Binary(IValue x, IValue y,
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
