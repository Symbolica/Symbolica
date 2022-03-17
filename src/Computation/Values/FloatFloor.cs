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

    public override FPExpr AsFloat(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var rounding = c.MkFPRTN();
            using var value = _value.AsFloat(context);
            return c.MkFPRoundToIntegral(rounding, value);
        });
    }

    public static IValue Create(IValue value)
    {
        return Unary(value,
            v => new ConstantSingle(MathF.Floor(v)),
            v => new ConstantDouble(Math.Floor(v)),
            v => new FloatFloor(v));
    }
}
