using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record SignedToFloat : Float
{
    private readonly IValue _value;

    private SignedToFloat(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override FPExpr AsFloat(ISolver solver)
    {
        using var rounding = solver.Context.MkFPRNE();
        using var value = _value.AsBitVector(solver);
        using var sort = Size.GetSort(solver);
        return solver.Context.MkFPToFP(rounding, value, sort, true);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as SignedToFloat);
    }

    public static IValue Create(Bits size, IValue value)
    {
        return value is IConstantValue v
            ? (uint) size switch
            {
                32U => v.AsSigned().ToSingle(),
                64U => v.AsSigned().ToDouble(),
                _ => new SignedToFloat(size, v)
            }
            : new SignedToFloat(size, value);
    }
}
