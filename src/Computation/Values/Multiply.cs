using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class Multiply : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Multiply(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkBVMul(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().Multiply(r.AsUnsigned())
            : new Multiply(left, right);
    }
}
