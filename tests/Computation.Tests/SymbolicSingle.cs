using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class SymbolicSingle : Float
{
    private readonly float _value;

    public SymbolicSingle(float value)
        : base((Bits) 32U)
    {
        _value = value;
    }

    public override FPExpr AsFloat(IContext context)
    {
        return context.CreateExpr(c => c.MkFP(_value, Size.GetSort(context)));
    }
}
