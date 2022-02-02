using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class SignedToFloat : Float
{
    private readonly IValue _value;

    private SignedToFloat(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(Context context)
    {
        return context.MkFPToFP(context.MkFPRNE(), _value.AsBitVector(context), Size.GetSort(context), true);
    }

    public static IValue Create(Bits size, IValue value)
    {
        return Value.Create(value,
            v => (uint) size switch
            {
                32U => v.AsSigned().ToSingle(),
                64U => v.AsSigned().ToDouble(),
                _ => new SignedToFloat(size, v)
            },
            v => new SignedToFloat(size, v));
    }
}
