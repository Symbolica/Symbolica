using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class FloatLessOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatLessOrEqual(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkFPLEq(_left.AsFloat(context), _right.AsFloat(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Value.Create(left, right,
            (l, r) => new ConstantBool(l <= r),
            (l, r) => new ConstantBool(l <= r),
            (l, r) => new FloatLessOrEqual(l, r));
    }
}
