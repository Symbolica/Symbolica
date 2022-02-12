using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class ArithmeticShiftRight : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private ArithmeticShiftRight(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkBVASHR(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsSigned().ShiftRight(r.AsUnsigned())
            : new ArithmeticShiftRight(left, right);
    }
}
