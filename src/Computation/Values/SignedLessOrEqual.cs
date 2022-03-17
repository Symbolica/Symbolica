using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record SignedLessOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedLessOrEqual(IValue left, IValue right)
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
            return c.MkBVSLE(left, right);
        });
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsSigned().LessOrEqual(r.AsSigned())
            : new SignedLessOrEqual(left, right);
    }
}
