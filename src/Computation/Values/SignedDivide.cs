﻿using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record SignedDivide : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedDivide(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVSDiv(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as SignedDivide);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsSigned().Divide(r.AsSigned())
            : new SignedDivide(left, right);
    }
}
