using Microsoft.Z3;

namespace Symbolica.Computation.Values.Symbolics;

internal sealed class Subtract : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    public Subtract(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkBVSub(_left.AsBitVector(context), _right.AsBitVector(context));
    }
}
