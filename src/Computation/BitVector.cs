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
        using var x = AsBitVector(context);
        using var y = context.MkBV(0U, (uint) Size);
        using var a = context.CreateExpr(c => c.MkEq(x, y));
        return context.CreateExpr(c => c.MkNot(a));
    }
}
