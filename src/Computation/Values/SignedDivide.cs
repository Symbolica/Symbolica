using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record SignedDivide : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedDivide(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c =>
        {
            using var left = _left.AsBitVector(context);
            using var right = _right.AsBitVector(context);
            return c.MkBVSDiv(left, right);
        });
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsSigned().Divide(r.AsSigned())
            : new SignedDivide(left, right);
    }
}
