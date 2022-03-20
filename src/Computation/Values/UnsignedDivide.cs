using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record UnsignedDivide : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private UnsignedDivide(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVUDiv(left, right);
    }

    public override bool Equals(IValue? other) => Equals(other as UnsignedDivide);

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().Divide(r.AsUnsigned())
            : new UnsignedDivide(left, right);
    }
}
