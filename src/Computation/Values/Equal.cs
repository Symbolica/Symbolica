using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record Equal : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Equal(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(IContext context)
    {
        var (left, right) = _left is Bool || _right is Bool
                ? (_left.AsBool(context) as Expr, _right.AsBool(context) as Expr)
                : (_left.AsBitVector(context), _right.AsBitVector(context));

        return context.CreateExpr(c =>
        {
            using (left)
            using (right)
                return c.MkEq(left, right);
        });
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().Equal(r.AsUnsigned())
            : new Equal(left, right);
    }
}
