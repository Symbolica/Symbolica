using Microsoft.Z3;

namespace Symbolica.Computation.Values;

internal sealed class Multiply : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Multiply(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkBVMul(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    private static IValue ShortCircuit(IValue left, IConstantValue right)
    {
        return right.AsUnsigned().IsOne
            ? left
            : Create(left, right);
    }

    private static IValue Create(IValue left, IConstantValue right)
    {
        return left switch
        {
            IConstantValue l => l.AsUnsigned().Multiply(right.AsUnsigned()),
            Multiply l => Create(l._left, Create(l._right, right)),
            _ => new Multiply(left, right)
        };
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, _) => ShortCircuit(right, l),
            (_, IConstantValue r) => ShortCircuit(left, r),
            _ => new Multiply(left, right)
        };
    }
}
