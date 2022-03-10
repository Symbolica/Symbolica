using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class Add : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Add(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkBVAdd(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    private static IValue ShortCircuit(IValue left, IConstantValue right)
    {
        return right.AsUnsigned().IsZero
            ? left
            : Create(left, right);
    }

    private static IValue Create(IValue left, IConstantValue right)
    {
        return left switch
        {
            IConstantValue l => l.AsUnsigned().Add(right.AsUnsigned()),
            Add l => Create(l._left, Create(l._right, right)),
            _ => new Add(left, right)
        };
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, _) => ShortCircuit(right, l),
            (_, IConstantValue r) => ShortCircuit(left, r),
            _ => new Add(left, right)
        };
    }
}
