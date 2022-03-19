using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record FloatSubtract : Float
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatSubtract(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override FPExpr AsFloat(ISolver solver)
    {
        using var rounding = solver.Context.MkFPRNE();
        using var left = _left.AsFloat(solver);
        using var right = _right.AsFloat(solver);
        return solver.Context.MkFPSub(rounding, left, right);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Binary(left, right,
            (l, r) => new ConstantSingle(l - r),
            (l, r) => new ConstantDouble(l - r),
            (l, r) => new FloatSubtract(l, r));
    }
}
