using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record UnsignedToFloat : Float
{
    private readonly IValue _value;

    private UnsignedToFloat(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var rounding = c.MkFPRNE();
            using var value = _value.AsBitVector(context);
            using var sort = Size.GetSort(context);
            return c.MkFPToFP(rounding, value, sort, false);
        });
    }

    public static IValue Create(Bits size, IValue value)
    {
        return value is IConstantValue v
            ? (uint) size switch
            {
                32U => v.AsUnsigned().ToSingle(),
                64U => v.AsUnsigned().ToDouble(),
                _ => new UnsignedToFloat(size, v)
            }
            : new UnsignedToFloat(size, value);
    }
}
