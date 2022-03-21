using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record ArithmeticShiftRight : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private ArithmeticShiftRight(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVASHR(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as ArithmeticShiftRight);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => l.AsSigned().ShiftRight(r.AsUnsigned()),
            (_, IConstantValue r) when r.AsUnsigned().IsZero => left,
            (IConstantValue l, _) when l.AsUnsigned().IsZero => l,
            _ => new ArithmeticShiftRight(left, right)
        };
    }
}
