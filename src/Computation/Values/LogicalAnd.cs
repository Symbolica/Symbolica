using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record LogicalAnd : Bool
{
    private readonly ImmutableHashSet<Bool> _values;

    private LogicalAnd(ImmutableHashSet<Bool> values)
    {
        _values = values;
    }

    public override ISet<IValue> Symbols => _values.SelectMany(c => c.Symbols).ToHashSet();

    public override BoolExpr AsBool(ISolver solver)
    {
        var exprs = _values.Select(x => x.AsBool(solver));
        try
        {
            return solver.Context.MkAnd(exprs);
        }
        finally
        {
            foreach (var expr in exprs)
                expr.Dispose();
        }
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as LogicalAnd);
    }

    public bool Equals(LogicalAnd? other)
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

    public Bool DistributeUnderOr(IValue value)
    {
        return _values
            .Select(conjunct => LogicalOr.Create(conjunct, value))
            .Aggregate(Create);
    }

    public Bool DeMorgan()
    {
        return _values.Select(LogicalNot.Create).Aggregate(LogicalOr.Create);
    }

    public static Bool Create(IValue left, IValue right)
    {
        static Bool Create(ImmutableHashSet<Bool> conjuncts, Bool value)
        {
            static Bool Create(ImmutableHashSet<Bool> conjuncts)
            {
                var cleaned = conjuncts.Remove(new ConstantBool(true));
                return conjuncts.Contains(new ConstantBool(false))
                    ? new ConstantBool(false)
                    : cleaned.Count == 0
                        ? new ConstantBool(true)
                        : cleaned.Count == 1
                            ? cleaned.Single()
                            : new LogicalAnd(cleaned);
            }

            static Bool Simplify(ImmutableHashSet<Bool> conjuncts, Bool value) =>
                value switch
                {
                    LogicalOr o => o.SimplifyUnderAnd(conjuncts),
                    _ => value
                };

            return Simplify(conjuncts, value) switch
            {
                LogicalAnd and => conjuncts.Aggregate(and as Bool, LogicalAnd.Create),
                LogicalOr o when conjuncts.Any(o.IsAbsorbedBy) => Create(conjuncts),
                var v when conjuncts.Contains(LogicalNot.Create(v)) => new ConstantBool(false),
                _ when conjuncts.Contains(LogicalNot.Create(value)) => new ConstantBool(false),
                var v => Create(
                    conjuncts
                        .Add(v)
                        .Where(c => !(c is LogicalOr or && or.IsAbsorbedBy(v)))
                        .Select(c => Simplify(new[] { v }.ToImmutableHashSet(), c))
                        .ToImmutableHashSet())
            };
        }

        return (Logical.Create(left), Logical.Create(right)) switch
        {
            (IConstantValue l, var r) => Create(new[] { r }.ToImmutableHashSet(), l.AsBool()),
            (var l, IConstantValue r) => Create(new[] { l }.ToImmutableHashSet(), r.AsBool()),
            (LogicalAnd and, var r) => Create(and._values, r),
            (var l, LogicalAnd and) => Create(and._values, l),
            var (l, r) => Create(new[] { l }.ToImmutableHashSet(), r)
        };
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is LogicalAnd a
            ? _values.IsSequenceEquivalentTo<(IValue, IValue), IValue>(a._values)
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
