using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal sealed class And : Integer
{
    private readonly IValue _left;
    private readonly IValue _right;

    private And(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(Context context)
    {
        return context.MkBVAND(_left.AsBitVector(context), _right.AsBitVector(context));
    }

    public override BoolExpr AsBool(Context context)
    {
        return context.MkAnd(_left.AsBool(context), _right.AsBool(context));
    }

    private static IValue ShortCircuit(ConstantUnsigned left, IValue right)
    {
        return left.IsZero
            ? left
            : left.Not().IsZero
                ? right
                : new And(left, right);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l
            ? right is IConstantValue r
                ? l.AsUnsigned().And(r.AsUnsigned())
                : ShortCircuit(l.AsUnsigned(), right)
            : right is IConstantValue c
                ? ShortCircuit(c.AsUnsigned(), left)
                : new And(left, right);
    }
}
