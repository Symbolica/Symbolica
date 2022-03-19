using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record FloatNegate : Float
{
    private readonly IValue _value;

    private FloatNegate(IValue value)
        : base(value.Size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var value = _value.AsFloat(context);
            return c.MkFPNeg(value);
        });
    }

    public static IValue Create(IValue value)
    {
        return Unary(value,
            v => new ConstantSingle(-v),
            v => new ConstantDouble(-v),
            v => new FloatNegate(v));
    }
}
