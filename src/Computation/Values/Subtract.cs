﻿using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record Subtract : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Subtract(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVSub(left, right);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().Subtract(r.AsUnsigned()),
            (_, IConstantValue r) when r.AsUnsigned().IsZero => left,
            _ => new Subtract(left, right)
        };
    }
}
