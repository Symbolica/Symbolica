using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class SymbolicDouble : Float
{
    private readonly double _value;

    public SymbolicDouble(double value)
        : base((Bits) 64U)
    {
        _value = value;
    }

    public override FPExpr AsFloat(Context context)
    {
        return context.MkFP(_value, context.MkFPSort64());
    }
}
