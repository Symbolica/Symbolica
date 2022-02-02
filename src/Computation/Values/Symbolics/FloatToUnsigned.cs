using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class FloatToUnsigned : BitVector
{
    private readonly IValue _value;

    private FloatToUnsigned(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkFPToBV(context.MkFPRTZ(), _value.AsFloat(context), (uint) Size, false);
    }

    public static IValue Create(Bits size, IValue value)
    {
        return Value.Create(value,
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            v => new FloatToUnsigned(size, v));
    }
}
