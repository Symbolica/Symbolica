using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record Add : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Add(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override IEnumerable<IValue> Children => new[] { _left, _right };

    public override string? PrintedValue => null;

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVAdd(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as Add);
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
            (Address<Bits> l, _) => Create(l.Aggregate(), right),
            (Address<Bytes> l, _) => Create(l.Aggregate(), right),
            (_, Address<Bits> r) => Create(left, r.Aggregate()),
            (_, Address<Bytes> r) => Create(left, r.Aggregate()),
            _ => new Add(left, right)
        };
    }
}
