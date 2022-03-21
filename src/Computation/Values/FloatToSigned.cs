using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record FloatToSigned : BitVector
{
    private readonly IValue _value;

    private FloatToSigned(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var rounding = solver.Context.MkFPRTZ();
        using var value = _value.AsFloat(solver);
        return solver.Context.MkFPToBV(rounding, value, (uint) Size, true);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as FloatToSigned);
    }

    public static IValue Create(Bits size, IValue value)
    {
        return Float.Unary(value,
            v => ConstantSigned.Create(size, (BigInteger) v),
            v => ConstantSigned.Create(size, (BigInteger) v),
            v => v is IRealValue r
                ? new RealToSigned(size, r)
                : new FloatToSigned(size, v));
    }
}
