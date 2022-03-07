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

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().Add(r.AsUnsigned()),
            (IConstantValue l, _) when l.AsUnsigned().IsZero => right,
            (_, IConstantValue r) when r.AsUnsigned().IsZero => left,
            _ => new Add(left, right)
        };
    }
}
