using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record ShiftLeft : BitVector
{
    private ShiftLeft(IValue left, IValue right)
        : base(left.Size)
    {
        Left = left;
        Right = right;
    }

    internal IValue Left { get; }

    internal IValue Right { get; }

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var left = Left.AsBitVector(solver);
        using var right = Right.AsBitVector(solver);
        return solver.Context.MkBVSHL(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as ShiftLeft);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().ShiftLeft(r.AsUnsigned()),
            (_, IConstantValue r) when r.AsUnsigned().IsZero => left,
            (IConstantValue l, _) when l.AsUnsigned().IsZero => l,
            (LogicalShiftRight lsr, _) when right.Equals(lsr.Right) => lsr.Left,
            _ => new ShiftLeft(left, right)
        };
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is ShiftLeft v
            ? Left.IsEquivalentTo(v.Left)
                .And(Right.IsEquivalentTo(v.Right))
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : Create(Left.Substitute(subs), Right.Substitute(subs));
    }
}
