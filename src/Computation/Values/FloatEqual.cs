using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class FloatEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatEqual(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkFPEq(_left.AsFloat(context), _right.AsFloat(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Value.Create(left, right,
            // ReSharper disable CompareOfFloatsByEqualityOperator
            (l, r) => new ConstantBool(l == r),
            (l, r) => new ConstantBool(l == r),
            // ReSharper restore CompareOfFloatsByEqualityOperator
            (l, r) => new FloatEqual(l, r));
    }
}
