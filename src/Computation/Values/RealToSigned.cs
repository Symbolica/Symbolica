using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record RealToSigned : BitVector
{
    private readonly IRealValue _value;

    public RealToSigned(Bits size, IRealValue value)
        : base(size)
    {
        _value = value;
    }

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var value = _value.AsReal(context);
            using var intValue = c.MkReal2Int(value);
            return c.MkInt2BV((uint) Size, intValue);
        });
    }
}
