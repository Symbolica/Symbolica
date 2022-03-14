using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract class Integer : IValue
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
        return context.CreateExpr(c => c.MkFPToFP(AsBitVector(context), Size.GetSort(context)));
    }
}
