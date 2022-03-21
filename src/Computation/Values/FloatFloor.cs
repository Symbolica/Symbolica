using System;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record FloatFloor : Float
{
    private readonly IValue _value;

    private FloatFloor(IValue value)
        : base(value.Size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(ISolver solver)
    {
        using var rounding = solver.Context.MkFPRTN();
        using var value = _value.AsFloat(solver);
        return solver.Context.MkFPRoundToIntegral(rounding, value);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as FloatFloor);
    }

    public static IValue Create(IValue value)
    {
        return Unary(value,
            v => new ConstantSingle(MathF.Floor(v)),
            v => new ConstantDouble(Math.Floor(v)),
            v => new FloatFloor(v));
    }
}
