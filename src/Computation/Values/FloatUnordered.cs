using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record FloatUnordered : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatUnordered(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(ISolver solver)
    {
        using var left = _left.AsFloat(solver);
        using var leftIsNaN = solver.Context.MkFPIsNaN(left);

        using var right = _right.AsFloat(solver);
        using var rightIsNaN = solver.Context.MkFPIsNaN(right);

        return solver.Context.MkOr(leftIsNaN, rightIsNaN);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as FloatUnordered);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Float.Binary(left, right,
            (l, r) => new ConstantBool(float.IsNaN(l) || float.IsNaN(r)),
            (l, r) => new ConstantBool(double.IsNaN(l) || double.IsNaN(r)),
            (l, r) => new FloatUnordered(l, r));
    }
}
