using System;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record FloatCeiling : Float
{
    private readonly IValue _value;

    private FloatCeiling(IValue value)
        : base(value.Size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(ISolver solver)
    {
        using var rounding = solver.Context.MkFPRTP();
        using var value = _value.AsFloat(solver);
        return solver.Context.MkFPRoundToIntegral(rounding, value);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as FloatCeiling);
    }

    public static IValue Create(IValue value)
    {
        return Unary(value,
            v => new ConstantSingle(MathF.Ceiling(v)),
            v => new ConstantDouble(Math.Ceiling(v)),
            v => new FloatCeiling(v));
    }
}
