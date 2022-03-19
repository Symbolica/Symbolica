using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract record BitVector : Integer
{
    protected BitVector(Bits size)
        : base(size)
    {
    }

    public sealed override BoolExpr AsBool(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var bitVector = AsBitVector(context);
            using var zero = c.MkBV(0U, (uint) Size);
            using var isZero = c.MkEq(bitVector, zero);
            return c.MkNot(isZero);
        });
    }
}
