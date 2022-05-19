using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record LogicalShiftRight : BitVector
{
    private LogicalShiftRight(IValue left, IValue right)
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
        return solver.Context.MkBVLSHR(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as LogicalShiftRight);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().ShiftRight(r.AsUnsigned()),
            (_, IConstantValue r) when r.AsUnsigned().IsZero => left,
            (IConstantValue l, _) when l.AsUnsigned().IsZero => l,
            (ShiftLeft sl, _) when right.Equals(sl.Right) => sl.Left,
            _ => new LogicalShiftRight(left, right)
        };
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is LogicalShiftRight v
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

    public override object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Left = Left.ToJson(),
            Right = Right.ToJson()
        };
    }
}
