using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract record Bool : Integer
{
    protected Bool()
        : base(Bits.One)
    {
    }

    public sealed override BitVecExpr AsBitVector(ISolver solver)
    {
        using var predicate = AsBool(solver);
        using var @true = solver.Context.MkBV(new[] { true });
        using var @false = solver.Context.MkBV(new[] { false });
        return (BitVecExpr) solver.Context.MkITE(predicate, @true, @false);
    }
}
