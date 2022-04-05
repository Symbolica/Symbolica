using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record FloatConvert : Float
{
    private readonly IValue _value;

    private FloatConvert(Size size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(ISolver solver)
    {
        using var rounding = solver.Context.MkFPRNE();
        using var value = _value.AsFloat(solver);
        using var sort = Size.GetSort(solver);
        return solver.Context.MkFPToFP(rounding, value, sort);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as FloatConvert);
    }

    public static IValue Create(Size size, IValue value)
    {
        return Unary(value,
            v => size.Bits switch
            {
                32U => new ConstantSingle(v),
                64U => new ConstantDouble(v),
                _ => new FloatConvert(size, new ConstantSingle(v))
            },
            v => size.Bits switch
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
