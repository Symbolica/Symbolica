using System;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

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

    private static IValue ShortCircuit(IValue left, ConstantUnsigned right)
    {
        return right.IsZero
            ? left
            : Create(left, right);
    }

    private static IValue Create(IValue left, ConstantUnsigned right)
    {
        return left switch
        {
            IConstantValue l => l.AsUnsigned().Add(right),
            Address<Bits> l => l.Add(right),
            Address<Bytes> l => l.Add(right),
            Add l => Create(l._left, Create(l._right, right)),
            _ => new Add(left, right)
        };
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, _) => ShortCircuit(right, l.AsUnsigned()),
            (_, IConstantValue r) => ShortCircuit(left, r.AsUnsigned()),
            (Address<Bits> l, Address<Bits> r) => Create(r.Aggregate(), l.Aggregate()),
            (Address<Bytes> l, Address<Bytes> r) => Create(r.Aggregate(), l.Aggregate()),
            (Address<Bits>, Address<Bytes>) => throw new Exception("Cannot add addresses of differrent size types"),
            (Address<Bytes>, Address<Bits>) => throw new Exception("Cannot add addresses of differrent size types"),
            (Address<Bits> l, _) => Create(l.Aggregate(), right),
            (Address<Bytes> l, _) => Create(l.Aggregate(), right),
            (_, Address<Bits> r) => Create(left, r.Aggregate()),
            (_, Address<Bytes> r) => Create(left, r.Aggregate()),
            _ => new Add(left, right)
        };
    }
}
