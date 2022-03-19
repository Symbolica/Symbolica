using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record UnsignedLess : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private UnsignedLess(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVULT(left, right);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().Less(r.AsUnsigned())
            : new UnsignedLess(left, right);
    }
}
