using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class ShiftLeft : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private ShiftLeft(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkBVSHL(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().ShiftLeft(r.AsUnsigned())
            : new ShiftLeft(left, right);
    }
}
