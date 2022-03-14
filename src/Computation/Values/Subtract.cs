using System.Collections.Generic;
using System.Diagnostics;
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

    public override IEnumerable<IValue> Children => new[] { _left, _right };

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(IContext context)
    {
        using var t1 = _left.AsBitVector(context);
        using var t2 = _right.AsBitVector(context);
        return context.CreateExpr(c => c.MkBVSub(t1, t2));
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
