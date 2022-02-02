﻿namespace Symbolica.Computation.Values;

internal static class NotEqual
{
    public static IValue Create(IValue left, IValue right)
    {
        return Value.Create(left, right,
            (l, r) => l.AsUnsigned().NotEqual(r.AsUnsigned()),
            (l, r) => Not.Create(Equal.Create(l, r)));
    }
}
