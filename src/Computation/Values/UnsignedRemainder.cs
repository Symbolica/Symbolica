using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class UnsignedRemainder : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private UnsignedRemainder(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkBVURem(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().Remainder(r.AsUnsigned())
            : new UnsignedRemainder(left, right);
    }
}
