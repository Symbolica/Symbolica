using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class FloatLess : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    public FloatLess(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkFPLt(_left.AsFloat(context), _right.AsFloat(context));
    }
}
