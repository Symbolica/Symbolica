using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class FloatConvert : Float
{
    private readonly IValue _value;

    private FloatConvert(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override IEnumerable<IValue> Children => new[] { _value };

    public override string? PrintedValue => null;

    public override FPExpr AsFloat(IContext context)
    {
        using var rm = context.CreateExpr(c => c.MkFPRNE());
        using var t = _value.AsFloat(context);
        using var sort = Size.GetSort(context);
        return context.CreateExpr(c => c.MkFPToFP(rm, t, sort));
    }

    public static IValue Create(Bits size, IValue value)
    {
        return Unary(value,
            v => (uint) size switch
            {
                32U => new ConstantSingle(v),
                64U => new ConstantDouble(v),
                _ => new FloatConvert(size, new ConstantSingle(v))
            },
            v => (uint) size switch
            {
                32U => new ConstantSingle((float) v),
                64U => new ConstantDouble(v),
                _ => new FloatConvert(size, new ConstantDouble(v))
            },
            v => v is IRealValue r
                ? new RealConvert(size, r)
                : new FloatConvert(size, v));
    }
}
