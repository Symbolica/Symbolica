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

    public override FPExpr AsFloat(ISolver solver)
    {
        using var rounding = solver.Context.MkFPRNE();
        using var value = _value.AsBitVector(solver);
        using var sort = Size.GetSort(solver);
        return solver.Context.MkFPToFP(rounding, value, sort, false);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as UnsignedToFloat);
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
