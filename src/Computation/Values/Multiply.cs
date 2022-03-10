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

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().Multiply(r.AsUnsigned()),
            (IConstantValue l, _) when l.AsUnsigned().IsOne => right,
            (_, IConstantValue r) when r.AsUnsigned().IsOne => left,
            _ => new Multiply(left, right)
        };
    }
}
