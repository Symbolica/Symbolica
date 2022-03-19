using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record FloatUnordered : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatUnordered(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(IContext context)
    {
        return context.CreateExpr(c =>
        {
            BoolExpr MkFPIsNaN(IValue value)
            {
                using var flt = value.AsFloat(context);
                return c.MkFPIsNaN(flt);
            }

            using var left = MkFPIsNaN(_left);
            using var right = MkFPIsNaN(_right);
            return c.MkOr(left, right);
        });
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Float.Binary(left, right,
            (l, r) => new ConstantBool(float.IsNaN(l) || float.IsNaN(r)),
            (l, r) => new ConstantBool(double.IsNaN(l) || double.IsNaN(r)),
            (l, r) => new FloatUnordered(l, r));
    }
}
