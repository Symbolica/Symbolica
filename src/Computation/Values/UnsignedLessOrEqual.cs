﻿using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record UnsignedLessOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private UnsignedLessOrEqual(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVULE(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as UnsignedLessOrEqual);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().LessOrEqual(r.AsUnsigned())
            : new UnsignedLessOrEqual(left, right);
    }
}
