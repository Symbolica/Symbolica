using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract record BitVector : Integer
{
    protected BitVector(Bits size)
        : base(size)
    {
    }

    public sealed override BoolExpr AsBool(ISolver solver)
    {
        using var bitVector = AsBitVector(solver);
        using var zero = solver.Context.MkBV(0U, (uint) Size);
        using var isZero = solver.Context.MkEq(bitVector, zero);
        return solver.Context.MkNot(isZero);
    }
}
