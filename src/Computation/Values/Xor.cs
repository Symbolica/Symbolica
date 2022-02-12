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

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkBVXOR(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    public override BoolExpr AsBool(IContext context)
    {
        return _left is Bool || _right is Bool
            ? context.CreateExpr(c => c.MkXor(_left.AsBool(context), _right.AsBool(context)))
            : AsBitVector(context).AsBool(context);
    }

    private static IValue ShortCircuit(IValue left, ConstantUnsigned right)
    {
        return right.IsZero
            ? left
            : right.Not().IsZero
                ? Not.Create(left)
                : Create(left, right);
    }

    private static IValue Create(IValue left, ConstantUnsigned right)
    {
        return left is IConstantValue cl
            ? cl.AsUnsigned().Xor(right)
            : new Xor(left, right);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return right is IConstantValue r
            ? ShortCircuit(left, r.AsUnsigned())
            : left is IConstantValue l
                ? ShortCircuit(right, l.AsUnsigned())
                : new Xor(left, right);
    }
}
