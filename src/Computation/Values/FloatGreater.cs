using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class FloatGreater : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatGreater(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkFPGt(_left.AsFloat(context), _right.AsFloat(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Float.Binary(left, right,
            (l, r) => new ConstantBool(l > r),
            (l, r) => new ConstantBool(l > r),
            (l, r) => new FloatGreater(l, r));
    }
}
