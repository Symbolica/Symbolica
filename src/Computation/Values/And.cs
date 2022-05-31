using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record And : BitVector
{
    private readonly IValue _left;
    private readonly IValue _right;

    private And(IValue left, IValue right)
        : base(left.Size)
    {
        _left = left;
        _right = right;
    }

    public override ISet<IValue> Symbols => _left.Symbols.Union(_right.Symbols).ToHashSet();

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVAND(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as And);
    }

    private static IValue ShortCircuit(IValue left, ConstantUnsigned right)
    {
        return right.IsZero
            ? right
            : right.Not().IsZero
                ? left
                : Create(left, right);
    }

    private static IValue Create(IValue left, ConstantUnsigned right)
    {
        return left switch
        {
            IConstantValue l => l.AsUnsigned().And(right),
            And l => Create(l._left, Create(l._right, right)),
            _ => new And(left, right)
        };
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, _) => ShortCircuit(right, l.AsUnsigned()),
            (_, IConstantValue r) => ShortCircuit(left, r.AsUnsigned()),
            _ when left.Equals(right) => left,
            _ => new And(left, right)
        };
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is And v
            ? _left.IsEquivalentTo(v._left)
                .And(_right.IsEquivalentTo(v._right))
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : Create(_left.Substitute(subs), _right.Substitute(subs));
    }

    public override object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Left = _left.ToJson(),
            Right = _right.ToJson()
        };
    }

    public override int GetEquivalencyHash(bool includeSubs)
    {
        return HashCode.Combine(
            GetType().Name,
            _left.GetEquivalencyHash(includeSubs),
            _right.GetEquivalencyHash(includeSubs));
    }

    public override IValue RenameSymbols(Func<string, string> renamer)
    {
        return Create(_left.RenameSymbols(renamer), _right.RenameSymbols(renamer));
    }
}
