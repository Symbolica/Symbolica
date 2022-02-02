using System;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class FloatCeiling : Float
{
    private readonly IValue _value;

    private FloatCeiling(IValue value)
        : base(value.Size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(Context context)
    {
        return context.MkFPRoundToIntegral(context.MkFPRTP(), _value.AsFloat(context));
    }

    public static IValue Create(IValue value)
    {
        return Value.Unary(value,
            v => new ConstantSingle(MathF.Ceiling(v)),
            v => new ConstantDouble(Math.Ceiling(v)),
            v => new FloatCeiling(v));
    }
}
