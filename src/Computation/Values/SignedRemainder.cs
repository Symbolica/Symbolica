﻿using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class SignedRemainder : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedRemainder(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkBVSRem(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Value.Create(left, right,
            (l, r) => l.AsSigned().Remainder(r.AsSigned()),
            (l, r) => new SignedRemainder(l, r));
    }
}