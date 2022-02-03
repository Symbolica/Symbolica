using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class Or : Integer
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Or(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkBVOR(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkOr(_left.AsBool(context), _right.AsBool(context));
    }

    private static IValue ShortCircuit(ConstantUnsigned left, IValue right)
    {
        return left.IsZero
            ? right
            : left.Not().IsZero
                ? left
                : new Or(left, right);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l
            ? right is IConstantValue r
                ? l.AsUnsigned().Or(r.AsUnsigned())
                : ShortCircuit(l.AsUnsigned(), right)
            : right is IConstantValue c
                ? ShortCircuit(c.AsUnsigned(), left)
                : new Or(left, right);
    }
}
