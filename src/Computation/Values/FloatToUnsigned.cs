using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record FloatToUnsigned : BitVector
{
    private readonly IValue _value;

    private FloatToUnsigned(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var rounding = solver.Context.MkFPRTZ();
        using var value = _value.AsFloat(solver);
        return solver.Context.MkFPToBV(rounding, value, (uint) Size, false);
    }

    public override bool Equals(IValue? other) => Equals(other as FloatToUnsigned);

    public static IValue Create(Bits size, IValue value)
    {
        return Float.Unary(value,
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            v => new FloatToUnsigned(size, v));
    }
}
