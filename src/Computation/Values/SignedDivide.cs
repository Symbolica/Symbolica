using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class SignedDivide : BitVector
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
        return context.Execute(c => c.MkBVSDiv(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsSigned().Divide(r.AsSigned())
            : new SignedDivide(left, right);
    }
}
