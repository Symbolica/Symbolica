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

    private static IValue ShortCircuit(IValue left, ConstantUnsigned right)
    {
        return right.IsZero
            ? left
            : right.Not().IsZero
                ? right
                : new Or(left, right);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l
            ? right is IConstantValue r
                ? l.AsUnsigned().Or(r.AsUnsigned())
                : ShortCircuit(right, l.AsUnsigned())
            : right is IConstantValue c
                ? ShortCircuit(left, c.AsUnsigned())
                : new Or(left, right);
    }
}
