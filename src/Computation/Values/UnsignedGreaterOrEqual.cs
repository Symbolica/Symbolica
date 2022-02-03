using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class UnsignedGreaterOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private UnsignedGreaterOrEqual(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkBVUGE(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().GreaterOrEqual(r.AsUnsigned())
            : new UnsignedGreaterOrEqual(left, right);
    }
}
