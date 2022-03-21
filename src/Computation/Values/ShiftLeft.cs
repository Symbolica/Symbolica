using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record ShiftLeft : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private ShiftLeft(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVSHL(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as ShiftLeft);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().ShiftLeft(r.AsUnsigned()),
            (_, IConstantValue r) when r.AsUnsigned().IsZero => left,
            (IConstantValue l, _) when l.AsUnsigned().IsZero => l,
            _ => new ShiftLeft(left, right)
        };
    }
}
