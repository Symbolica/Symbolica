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

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkFPToBV(c.MkFPRTZ(), _value.AsFloat(context), (uint) Size, false));
    }

    public static IValue Create(Bits size, IValue value)
    {
        return Float.Unary(value,
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            v => new FloatToUnsigned(size, v));
    }
}
