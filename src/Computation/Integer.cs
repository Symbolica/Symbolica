using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract record Integer : IValue
{
    protected Integer(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }

    public abstract BitVecExpr AsBitVector(IContext context);
    public abstract BoolExpr AsBool(IContext context);

    public FPExpr AsFloat(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var bitVector = AsBitVector(context);
            using var sort = Size.GetSort(context);
            return c.MkFPToFP(bitVector, sort);
        });
    }
}
