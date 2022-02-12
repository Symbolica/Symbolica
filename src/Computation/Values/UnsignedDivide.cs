using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class UnsignedDivide : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private UnsignedDivide(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkBVUDiv(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().Divide(r.AsUnsigned())
            : new UnsignedDivide(left, right);
    }
}
