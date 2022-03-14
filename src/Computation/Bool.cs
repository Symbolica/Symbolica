using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract class Bool : Integer
{
    protected Bool()
        : base(Bits.One)
    {
    }

    public sealed override BitVecExpr AsBitVector(IContext context)
    {
        using var t1 = AsBool(context);
        using var t2 = context.CreateExpr(c => c.MkBV(new[] { true }));
        using var t3 = context.CreateExpr(c => c.MkBV(new[] { false }));
        return context.CreateExpr(c => (BitVecExpr) c.MkITE(t1, t2, t3));
    }
}
