using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class FloatConvert : Float
{
    private readonly IValue _value;

    private FloatConvert(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(Context context)
    {
        return context.MkFPToFP(context.MkFPRNE(), _value.AsFloat(context), Size.GetSort(context));
    }

    public static IValue Create(Bits size, IValue value)
    {
        return Value.Unary(value,
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
