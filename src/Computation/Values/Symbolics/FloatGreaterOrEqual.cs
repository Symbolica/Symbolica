using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class FloatGreaterOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatGreaterOrEqual(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkFPGEq(_left.AsFloat(context), _right.AsFloat(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Value.Binary(left, right,
            (l, r) => new ConstantBool(l >= r),
            (l, r) => new ConstantBool(l >= r),
            (l, r) => new FloatGreaterOrEqual(l, r));
    }
}
