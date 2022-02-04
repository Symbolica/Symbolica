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

    private static IValue ShortCircuit(IValue left, ConstantUnsigned right)
    {
        return right.IsZero
            ? left
            : right.Not().IsZero
                ? Not.Create(left)
                : new Xor(left, right);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l
            ? right is IConstantValue r
                ? l.AsUnsigned().Xor(r.AsUnsigned())
                : ShortCircuit(right, l.AsUnsigned())
            : right is IConstantValue c
                ? ShortCircuit(left, c.AsUnsigned())
                : new Xor(left, right);
    }
}
