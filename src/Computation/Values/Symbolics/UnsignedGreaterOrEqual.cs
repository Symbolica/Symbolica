using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class UnsignedGreaterOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    public UnsignedGreaterOrEqual(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkBVUGE(_left.AsBitVector(context), _right.AsBitVector(context));
    }
}
