using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record And : BitVector
{
    private readonly ImmutableHashSet<IValue> _values;
    private readonly ConstantUnsigned? _constant;

    private And(ImmutableHashSet<IValue> values, ConstantUnsigned? constant)
        : base(values.First().Size)
    {
        _values = values;
        _constant = constant;
    }

    private IEnumerable<IValue> AllValues
        => _values.Concat(_constant is null ? Enumerable.Empty<IValue>() : new[] { _constant });

    public override ISet<IValue> Symbols => AllValues.SelectMany(c => c.Symbols).ToHashSet();

    public override BitVecExpr AsBitVector(ISolver solver)
    {
        var exprs = AllValues.Select(x => x.AsBitVector(solver));
        try
        {
            return exprs.Aggregate(solver.Context.MkBVAND);
        }
        finally
        {
            foreach (var expr in exprs)
                expr.Dispose();
        }
    }

    public override bool Equals(IValue? other)
    {
        return Equals(other as And);
    }

    public bool Equals(And? other)
    {
        return other is not null && AllValues.SequenceEqual(other.AllValues);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var value in AllValues)
            hash.Add(value);
        return hash.ToHashCode();
    }

    private IValue AndValue(IValue value)
        => value switch
        {
            IConstantValue c
                => Create(
                    _values,
                    _constant is null
                        ? c.AsUnsigned()
                        : _constant.And(c.AsUnsigned())),
            _ => new And(_values.Add(value), _constant)
        };

    private static ConstantUnsigned Create(IConstantValue left, IConstantValue right)
    {
        return left.AsUnsigned().And(right.AsUnsigned());
    }

    private static IValue Create(IEnumerable<IValue> values)
    {
        return Create(
            values.Where(v => v is not IConstantValue).ToImmutableHashSet(),
            values.OfType<IConstantValue>().Aggregate(Create).AsUnsigned());
    }

    private static IValue Create(ImmutableHashSet<IValue> values, ConstantUnsigned constant)
    {
        return constant.IsZero
            ? constant
            : constant.Not().IsZero
                ? values.Count == 1
                    ? values.Single()
                    : new And(values, null)
                : new And(values, constant);
    }

    public static IValue Create(IValue left, IValue right)
    {
        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => Create(l, r),
            (And a, _) => a.AndValue(right),
            (_, And a) => a.AndValue(left),
            (IConstantValue l, _) => Create(new[] { right }.ToImmutableHashSet(), l.AsUnsigned()),
            (_, IConstantValue r) => Create(new[] { left }.ToImmutableHashSet(), r.AsUnsigned()),
            _ when left.Equals(right) => left,
            _ => new And(new[] { left, right }.ToImmutableHashSet(), null)
        };
    }

    public override (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is And a
            ? AllValues.IsSequenceEquivalentTo<(IValue, IValue), IValue>(a.AllValues)
            : (new(), false);
    }

    public override IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return subs.TryGetValue(this, out var sub)
            ? sub
            : Create(AllValues.Select(v => v.Substitute(subs)));
    }

    public override object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Constant = _constant?.ToJson(),
            Values = _values.Select(v => v.ToJson()).ToArray()
        };
    }

    public override int GetEquivalencyHash()
    {
        var valuesHash = new HashCode();
        foreach (var value in AllValues)
            valuesHash.Add(value.GetEquivalencyHash());

        return HashCode.Combine(
            GetType().Name,
            valuesHash.ToHashCode());
    }

    public override IValue RenameSymbols(Func<string, string> renamer)
    {
        return Create(AllValues.Select(v => v.RenameSymbols(renamer)));
    }
}
