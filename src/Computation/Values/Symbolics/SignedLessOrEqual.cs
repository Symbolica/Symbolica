﻿using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class SignedLessOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedLessOrEqual(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkBVSLE(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Value.Create(left, right,
            (l, r) => l.AsSigned().LessOrEqual(r.AsSigned()),
            (l, r) => new SignedLessOrEqual(l, r));
    }
}
