using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class FloatUnordered : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    public FloatUnordered(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkOr(context.MkFPIsNaN(_left.AsFloat(context)), context.MkFPIsNaN(_right.AsFloat(context)));
    }
}
