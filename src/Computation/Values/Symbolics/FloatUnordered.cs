using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class FloatUnordered : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatUnordered(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkOr(context.MkFPIsNaN(_left.AsFloat(context)), context.MkFPIsNaN(_right.AsFloat(context)));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Value.Create(left, right,
            (l, r) => new ConstantBool(float.IsNaN(l) || float.IsNaN(r)),
            (l, r) => new ConstantBool(double.IsNaN(l) || double.IsNaN(r)),
            (l, r) => new FloatUnordered(l, r));
    }
}
