using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record SignedLessOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedLessOrEqual(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVSLE(left, right);
    }

    public override bool Equals(IValue? other) => Equals(other as SignedLessOrEqual);

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsSigned().LessOrEqual(r.AsSigned())
            : new SignedLessOrEqual(left, right);
    }
}
