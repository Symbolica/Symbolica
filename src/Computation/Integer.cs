using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal abstract record Integer : IValue
{
    protected Integer(Size size)
    {
        Size = size;
    }

    public Size Size { get; }

    public abstract BitVecExpr AsBitVector(ISolver solver);
    public abstract BoolExpr AsBool(ISolver solver);
    public abstract bool Equals(IValue? other);

    public FPExpr AsFloat(ISolver solver)
    {
        using var bitVector = AsBitVector(solver);
        using var sort = Size.GetSort(solver);
        return solver.Context.MkFPToFP(bitVector, sort);
    }
}
