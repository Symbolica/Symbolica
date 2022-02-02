using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract class BitVector : Integer
{
    protected BitVector(Bits size)
        : base(size)
    {
    }

    public sealed override BoolExpr AsBool(Context context)
    {
        return context.MkNot(context.MkEq(AsBitVector(context), context.MkBV(0U, (uint) Size)));
    }
}
