using System.Collections.Generic;
using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record SignedRemainder : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedRemainder(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<IValue> Children => new[] { _left, _right };

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVSRem(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as SignedRemainder);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsSigned().Remainder(r.AsSigned())
            : new SignedRemainder(left, right);
    }
}
