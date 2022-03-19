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

    public override FPExpr AsFloat(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var rounding = c.MkFPRTP();
            using var value = _value.AsFloat(context);
            return c.MkFPRoundToIntegral(rounding, value);
        });
    }

    public static IValue Create(IValue value)
    {
        return Unary(value,
            v => new ConstantSingle(MathF.Ceiling(v)),
            v => new ConstantDouble(Math.Ceiling(v)),
            v => new FloatCeiling(v));
    }
}
