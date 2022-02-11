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
        return context.CreateExpr(c => (BitVecExpr) c.MkITE(AsBool(context),
            c.MkBV(new[] {true}),
            c.MkBV(new[] {false})));
    }
}
