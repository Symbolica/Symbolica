using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class FloatNegate : Float
{
    private readonly IValue _value;

    private FloatNegate(IValue value)
        : base(value.Size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(Context context)
    {
        return context.MkFPNeg(_value.AsFloat(context));
    }

    public static IValue Create(IValue value)
    {
        return Value.Create(value,
            v => new ConstantSingle(-v),
            v => new ConstantDouble(-v),
            v => new FloatNegate(v));
    }
}
