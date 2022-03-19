using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed record FloatGreaterOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatGreaterOrEqual(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var left = _left.AsFloat(context);
            using var right = _right.AsFloat(context);
            return c.MkFPGEq(left, right);
        });
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Float.Binary(left, right,
            (l, r) => new ConstantBool(l >= r),
            (l, r) => new ConstantBool(l >= r),
            (l, r) => new FloatGreaterOrEqual(l, r));
    }
}
