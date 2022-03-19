using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract record Bool : Integer
{
    protected Bool()
        : base(Bits.One)
    {
    }

    public sealed override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var predicate = AsBool(context);
            using var @true = c.MkBV(new[] { true });
            using var @false = c.MkBV(new[] { false });
            return (BitVecExpr) c.MkITE(predicate, @true, @false);
        });
    }
}
