using System;

namespace Symbolica.Computation;

internal static class Value
{
    public static IValue Create(IValue x,
        Func<IConstantValue, IValue> constant,
        Func<IValue, IValue> symbolic)
    {
        return x is IConstantValue cx
            ? constant(cx)
            : symbolic(x);
    }

    public static IValue Create(IValue x, IValue y,
        Func<IConstantValue, IConstantValue, IValue> constant,
        Func<IValue, IValue, IValue> symbolic)
    {
        return x is IConstantValue cx && y is IConstantValue cy
            ? constant(cx, cy)
            : symbolic(x, y);
    }

    public static IValue Create(IValue x, IValue y, IValue z,
        Func<IConstantValue, IConstantValue, IConstantValue, IValue> constant,
        Func<IValue, IValue, IValue, IValue> symbolic)
    {
        return x is IConstantValue cx && y is IConstantValue cy && z is IConstantValue cz
            ? constant(cx, cy, cz)
            : symbolic(x, y, z);
    }

    public static IValue Create(IValue x,
        Func<float, IValue> constantSingle,
        Func<double, IValue> constantDouble,
        Func<IValue, IValue> symbolic)
    {
        return Create(x,
            a => (uint) a.Size switch
            {
                32U => constantSingle(a.AsSingle()),
                64U => constantDouble(a.AsDouble()),
                _ => symbolic(a)
            },
            symbolic);
    }

    public static IValue Create(IValue x, IValue y,
        Func<float, float, IValue> constantSingle,
        Func<double, double, IValue> constantDouble,
        Func<IValue, IValue, IValue> symbolic)
    {
        return Create(x, y,
            (a, b) => (uint) a.Size switch
            {
                32U => constantSingle(a.AsSingle(), b.AsSingle()),
                64U => constantDouble(a.AsDouble(), b.AsDouble()),
                _ => symbolic(a, b)
            },
            symbolic);
    }
}
