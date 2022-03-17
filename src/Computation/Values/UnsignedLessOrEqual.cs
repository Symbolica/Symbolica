using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record UnsignedLessOrEqual : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private UnsignedLessOrEqual(IValue left, IValue right)
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
            return c.MkBVULE(left, right);
        });
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().LessOrEqual(r.AsUnsigned())
            : new UnsignedLessOrEqual(left, right);
    }
}
