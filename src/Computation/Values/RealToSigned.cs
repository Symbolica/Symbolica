using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class RealToSigned : BitVector
{
    private readonly IRealValue _value;

    public RealToSigned(Bits size, IRealValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkInt2BV((uint) Size, context.MkReal2Int(_value.AsReal(context)));
    }
}
