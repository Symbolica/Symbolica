using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class FloatGreater : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    public FloatGreater(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkFPGt(_left.AsFloat(context), _right.AsFloat(context));
    }
}
