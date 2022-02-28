using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record Multiply : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private Multiply(IValue left, IValue right)
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
        return solver.Context.MkBVMul(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as Multiply);
    }

    private static IValue ShortCircuit(IValue left, ConstantUnsigned right)
    {
        return right.IsZero
            ? right
            : right.IsOne
                ? left
                : Create(left, right);
    }

    private static IValue Create(IValue left, ConstantUnsigned right)
    {
        return left switch
        {
            IConstantValue l => l.AsUnsigned().Multiply(right),
            Multiply l => Create(l._left, Create(l._right, right)),
            Address<Bits> l => l.Multiply(right),
            Address<Bytes> l => l.Multiply(right),
            _ => new Multiply(left, right)
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
            _ => new Multiply(left, right)
        };
    }
}
