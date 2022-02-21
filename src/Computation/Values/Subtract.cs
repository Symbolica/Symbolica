using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed class Subtract : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Subtract(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override BitVecExpr AsBitVector(IContext context)
    {
        return context.CreateExpr(c => c.MkBVSub(_left.AsBitVector(context), _right.AsBitVector(context)));
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().Subtract(r.AsUnsigned()),
            (_, IConstantValue r) when r.AsUnsigned().IsZero => left,
            (Address<Bits> l, IConstantValue r) => l.Subtract(r),
            (Address<Bytes> l, IConstantValue r) => l.Subtract(r),
            (IConstantValue l, Address<Bits> r) => r.Negate().Add(l),
            (IConstantValue l, Address<Bytes> r) => r.Negate().Add(l),
            (Address<Bits> l, _) => Create(l.Aggregate(), right),
            (Address<Bytes> l, _) => Create(l.Aggregate(), right),
            (_, Address<Bits> r) => Create(left, r.Aggregate()),
            (_, Address<Bytes> r) => Create(left, r.Aggregate()),
            _ => new Subtract(left, right)
        };
    }
}
