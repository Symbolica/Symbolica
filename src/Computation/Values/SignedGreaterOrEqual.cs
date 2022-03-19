using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record SignedGreaterOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedGreaterOrEqual(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override BoolExpr AsBool(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var left = _left.AsBitVector(context);
            using var right = _right.AsBitVector(context);
            return c.MkBVSGE(left, right);
        });
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsSigned().GreaterOrEqual(r.AsSigned())
            : new SignedGreaterOrEqual(left, right);
    }
}
