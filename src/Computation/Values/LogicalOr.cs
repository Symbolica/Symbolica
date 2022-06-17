using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record LogicalOr : Bool
{
    private readonly ImmutableHashSet<Bool> _values;

    private LogicalOr(ImmutableHashSet<Bool> values)
    {
        _values = values;
    }

    public override ISet<IValue> Symbols => _values.SelectMany(c => c.Symbols).ToHashSet();

    public override BoolExpr AsBool(ISolver solver)
    {
        var exprs = _values.Select(x => x.AsBool(solver));
        try
        {
            return solver.Context.MkOr(exprs);
        }
        finally
        {
            foreach (var expr in exprs)
                expr.Dispose();
        }
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as LogicalOr);
    }

    public bool Equals(LogicalOr? other)
    {
        return other is not null && _values.SetEquals(other._values);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var value in _values)
            hash.Add(value);
        return hash.ToHashCode();
    }

    public bool IsAbsorbedBy(Bool value)
    {
        return _values.Contains(value);
    }

    public Bool SimplifyUnderAnd(ImmutableHashSet<Bool> conjuncts)
    {
        return _values
            .Except(conjuncts.Select(LogicalNot.Create).ToImmutableHashSet())
            .Aggregate(new ConstantBool(false) as Bool, Create);
    }

    public Bool DeMorgan()
    {
        return _values.Select(LogicalNot.Create).Aggregate(LogicalAnd.Create);
    }

    public static Bool Create(IValue left, IValue right)
    {
        static Bool Create(ImmutableHashSet<Bool> disjuncts, Bool value)
        {
            static Bool Create(ImmutableHashSet<Bool> disjuncts)
                => disjuncts.Count == 1
                    ? disjuncts.Single()
                    : new LogicalOr(disjuncts);
            return value switch
            {
                LogicalOr or => disjuncts.Aggregate(or as Bool, LogicalOr.Create),
                _ when disjuncts.Contains(LogicalNot.Create(value)) => new ConstantBool(true),
                _ => Create(disjuncts.Add(value))
            };
        }

        static Bool ShortCircuit(Bool left, ConstantBool right) =>
            right ? right : left;

        return (Logical.Create(left), Logical.Create(right)) switch
        {
            (IConstantValue l, var r) => ShortCircuit(r, l.AsBool()),
            (var l, IConstantValue r) => ShortCircuit(l, r.AsBool()),
            (LogicalAnd l, LogicalAnd r) when l.Equals(r) => l,
            (LogicalAnd a, _) => a.DistributeUnderOr(right),
            (_, LogicalAnd a) => a.DistributeUnderOr(left),
            (LogicalOr or, var r) => Create(or._values, r),
            (var l, LogicalOr or) => Create(or._values, l),
            var (l, r) => Create(new[] { l }.ToImmutableHashSet(), r)
        };
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is LogicalOr o
            ? _values.IsSequenceEquivalentTo<(IValue, IValue), IValue>(o._values)
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : _values.Select(v => v.Substitute(subs)).Aggregate(Create);
    }

    public override object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Values = _values.Select(v => v.ToJson()).ToArray()
        };
    }

    public override int GetEquivalencyHash()
    {
        var valuesHash = new HashCode();
        foreach (var value in _values)
            valuesHash.Add(value.GetEquivalencyHash());

        return HashCode.Combine(
            GetType().Name,
            valuesHash.ToHashCode());
    }

    public override IValue RenameSymbols(Func<string, string> renamer)
    {
        return _values.Select(v => v.RenameSymbols(renamer)).Aggregate(Create);
    }
}
