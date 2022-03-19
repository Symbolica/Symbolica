using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed record SymbolicDouble : Float
{
    private readonly double _value;

    public SymbolicDouble(double value)
        : base((Bits) 64U)
    {
        _value = value;
    }

    public override FPExpr AsFloat(ISolver solver)
    {
        using var sort = Size.GetSort(solver);
        return solver.Context.MkFP(_value, sort);
    }
}
