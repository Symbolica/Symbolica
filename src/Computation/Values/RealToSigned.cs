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

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var value = _value.AsReal(solver);
        using var intValue = solver.Context.MkReal2Int(value);
        return solver.Context.MkInt2BV((uint) Size, intValue);
    }
}
