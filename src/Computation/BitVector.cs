using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract class BitVector : Integer
{
    protected BitVector(Bits size)
        : base(size)
    {
    }

    public sealed override BoolExpr AsBool(IContext context)
    {
        return context.CreateExpr(c => c.MkNot(c.MkEq(AsBitVector(context), c.MkBV(0U, (uint) Size))));
    }
}
