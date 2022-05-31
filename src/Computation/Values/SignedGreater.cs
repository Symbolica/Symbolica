using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record SignedGreater : Bool
{
    private readonly IValue _left;
    private readonly IValue _right;

    private SignedGreater(IValue left, IValue right)
    {
        _left = left;
        _right = right;
    }

    public override ISet<IValue> Symbols => _left.Symbols.Union(_right.Symbols).ToHashSet();

    public override BoolExpr AsBool(ISolver solver)
    {
        using var left = _left.AsBitVector(solver);
        using var right = _right.AsBitVector(solver);
        return solver.Context.MkBVSGT(left, right);
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as SignedGreater);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsSigned().Greater(r.AsSigned())
            : new SignedGreater(left, right);
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is SignedGreater v
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
