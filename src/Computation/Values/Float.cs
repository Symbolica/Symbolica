using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal abstract class Float : IValue
{
    protected Float(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }

    public BitVecExpr AsBitVector(Context context)
    {
        return context.MkFPToIEEEBV(AsFloat(context));
    }

    public BoolExpr AsBool(Context context)
    {
        return context.MkNot(context.MkFPIsZero(AsFloat(context)));
    }

    public abstract FPExpr AsFloat(Context context);
}
