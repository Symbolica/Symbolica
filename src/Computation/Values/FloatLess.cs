using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record FloatLess : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private FloatLess(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override ISet<IValue> Symbols => _left.Symbols.Union(_right.Symbols).ToHashSet();

    public override BoolExpr AsBool(ISolver solver)
    {
        using var left = _left.AsFloat(solver);
        using var right = _right.AsFloat(solver);
        return solver.Context.MkFPLt(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as FloatLess);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return Float.Binary(left, right,
            (l, r) => new ConstantBool(l < r),
            (l, r) => new ConstantBool(l < r),
            (l, r) => new FloatLess(l, r));
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is FloatLess v
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
}
