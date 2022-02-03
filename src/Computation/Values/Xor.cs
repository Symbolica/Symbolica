using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class Xor : Integer
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Xor(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkBVXOR(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkXor(_left.AsBool(context), _right.AsBool(context));
    }

    private static IValue ShortCircuit(ConstantUnsigned left, IValue right)
    {
        return left.IsZero
            ? right
            : left.Not().IsZero
                ? Not.Create(right)
                : new Xor(left, right);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l
            ? right is IConstantValue r
                ? l.AsUnsigned().Xor(r.AsUnsigned())
                : ShortCircuit(l.AsUnsigned(), right)
            : right is IConstantValue c
                ? ShortCircuit(c.AsUnsigned(), left)
                : new Xor(left, right);
    }
}
