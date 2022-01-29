using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class FloatToUnsigned : BitVector
{
    private readonly IValue _value;

    public FloatToUnsigned(Bits size, IValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkFPToBV(context.MkFPRTZ(), _value.AsFloat(context), (uint) Size, false);
    }
}
