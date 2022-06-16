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
    private readonly ImmutableHashSet<IValue> _values;

    private LogicalOr(ImmutableHashSet<IValue> values)
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
        return other is not null && _values.SequenceEqual(other._values);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var value in _values)
            hash.Add(value);
        return hash.ToHashCode();
    }

    public bool IsAbsorbedBy(IValue value)
    {
        return _values.Contains(value);
    }

    public IValue DeMorgan()
    {
        return _values.Select(LogicalNot.Create).Aggregate(LogicalAnd.Create);
    }

    public static IValue Create(IValue left, IValue right)
    {
        static IValue Create(ImmutableHashSet<IValue> disjuncts, IValue value)
        {
            static IValue Create(ImmutableHashSet<IValue> disjuncts)
                => disjuncts.Count == 1
                    ? disjuncts.Single()
                    : new LogicalOr(disjuncts);
            return value switch
            {
                LogicalOr or => disjuncts.Aggregate(or as IValue, LogicalOr.Create),
                _ when disjuncts.Contains(LogicalNot.Create(value)) => new ConstantBool(true),
                _ => Create(disjuncts.Add(value))
            };
        }

        return (left, right) switch
        {
            (IConstantValue l, _) => l.AsBool() ? l : right,
            (_, IConstantValue r) => r.AsBool() ? r : left,
            (LogicalAnd l, LogicalAnd r) when l.Equals(r) => l,
            (LogicalAnd a, _) => a.DistributeUnderOr(right),
            (_, LogicalAnd a) => a.DistributeUnderOr(left),
            (LogicalOr or, _) => Create(or._values, right),
            (_, LogicalOr or) => Create(or._values, left),
            _ => Create(new[] { left }.ToImmutableHashSet(), right)
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
