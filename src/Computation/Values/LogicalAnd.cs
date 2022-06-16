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
    private readonly ImmutableHashSet<IValue> _values;

    private LogicalAnd(ImmutableHashSet<IValue> values)
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
        return other is not null && _values.SequenceEqual(other._values);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var value in _values)
            hash.Add(value);
        return hash.ToHashCode();
    }

    public IValue DistributeUnderOr(IValue value)
    {
        return _values
            .Select(conjunct => LogicalOr.Create(conjunct, value))
            .Aggregate(Create);
    }

    public IValue DeMorgan()
    {
        return _values.Select(LogicalNot.Create).Aggregate(LogicalOr.Create);
    }

    public static IValue Create(IValue left, IValue right)
    {
        static IValue Create(ImmutableHashSet<IValue> conjuncts, IValue value)
        {
            static IValue Create(ImmutableHashSet<IValue> conjuncts)
                => conjuncts.Count == 1
                    ? conjuncts.Single()
                    : new LogicalAnd(conjuncts);
            return value switch
            {
                LogicalAnd and => conjuncts.Aggregate(and as IValue, LogicalAnd.Create),
                LogicalOr o when conjuncts.Any(o.IsAbsorbedBy) => Create(conjuncts),
                _ when conjuncts.Contains(LogicalNot.Create(value)) => new ConstantBool(false),
                _ => Create(
                    conjuncts
                        .Add(value)
                        .Where(v => !(v is LogicalOr or && or.IsAbsorbedBy(value)))
                        .ToImmutableHashSet())
            };
        }

        return (left, right) switch
        {
            (IConstantValue l, _) => l.AsBool() ? right : l,
            (_, IConstantValue r) => r.AsBool() ? left : r,
            (LogicalAnd and, _) => Create(and._values, right),
            (_, LogicalAnd and) => Create(and._values, left),
            (LogicalOr or, _) when or.IsAbsorbedBy(right) => right,
            (_, LogicalOr or) when or.IsAbsorbedBy(left) => left,
            _ => Create(new[] { left }.ToImmutableHashSet(), right)
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
