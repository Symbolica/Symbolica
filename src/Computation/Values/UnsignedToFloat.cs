using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class UnsignedToFloat : Float
{
    private readonly IValue _value;

    private UnsignedToFloat(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(Context context)
    {
        return context.MkFPToFP(context.MkFPRNE(), _value.AsBitVector(context), Size.GetSort(context), false);
    }

    public static IValue Create(Bits size, IValue value)
    {
        return Value.Create(value,
            v => (uint) size switch
            {
                32U => v.AsUnsigned().ToSingle(),
                64U => v.AsUnsigned().ToDouble(),
                _ => new UnsignedToFloat(size, v)
            },
            v => new UnsignedToFloat(size, v));
    }
}
