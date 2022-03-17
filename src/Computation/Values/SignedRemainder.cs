using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed record SignedRemainder : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedRemainder(IValue left, IValue right)
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
            return c.MkBVSRem(left, right);
        });
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsSigned().Remainder(r.AsSigned())
            : new SignedRemainder(left, right);
    }
}
