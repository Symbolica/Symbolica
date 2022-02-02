using System.Numerics;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class FloatToSigned : BitVector
{
    private readonly IValue _value;

    private FloatToSigned(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkFPToBV(context.MkFPRTZ(), _value.AsFloat(context), (uint) Size, true);
    }

    public static IValue Create(Bits size, IValue value)
    {
        return Value.Create(value,
            v => ConstantSigned.Create(size, (BigInteger) v),
            v => ConstantSigned.Create(size, (BigInteger) v),
            v => v is IRealValue r
                ? new RealToSigned(size, r)
                : new FloatToSigned(size, v));
    }
}
