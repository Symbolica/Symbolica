using System;
using System.Collections.Generic;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal sealed record Select : IValue
{
    private readonly IValue _falseValue;
    private readonly IValue _predicate;
    private readonly IValue _trueValue;

    private Select(IValue predicate, IValue trueValue, IValue falseValue)
    {
        _predicate = predicate;
        _trueValue = trueValue;
        _falseValue = falseValue;
    }

    public Bits Size => _trueValue.Size;

    public BitVecExpr AsBitVector(ISolver solver)
    {
        using var predicate = _predicate.AsBool(solver);
        using var trueValue = _trueValue.AsBitVector(solver);
        using var falseValue = _falseValue.AsBitVector(solver);
        return (BitVecExpr) solver.Context.MkITE(predicate, trueValue, falseValue);
    }

    public BoolExpr AsBool(ISolver solver)
    {
        using var predicate = _predicate.AsBool(solver);
        using var trueValue = _trueValue.AsBool(solver);
        using var falseValue = _falseValue.AsBool(solver);
        return (BoolExpr) solver.Context.MkITE(predicate, trueValue, falseValue);
    }

    public FPExpr AsFloat(ISolver solver)
    {
        using var predicate = _predicate.AsBool(solver);
        using var trueValue = _trueValue.AsFloat(solver);
        using var falseValue = _falseValue.AsFloat(solver);
        return (FPExpr) solver.Context.MkITE(predicate, trueValue, falseValue);
    }

    public bool Equals(IValue? other)
    {
        return Equals(other as Select);
    }

    public bool TryMerge(IValue value, out IValue merged)
    {
        merged = value;
        return Equals(value);
    }

    public static IValue Create(IValue predicate, IValue trueValue, IValue falseValue)
    {
        return (predicate, trueValue, falseValue) switch
        {
            (IConstantValue p, _, _) => p.AsBool() ? trueValue : falseValue,
            _ when trueValue.Equals(falseValue) => trueValue,
            _ => new Select(predicate, trueValue, falseValue)
        };
    }

    public (HashSet<(IValue, IValue)> subs, bool) IsEquivalentTo(IValue other)
    {
        return other is Select s
            ? _predicate.IsEquivalentTo(s._predicate)
                .And(_trueValue.IsEquivalentTo(s._trueValue))
                .And(_falseValue.IsEquivalentTo(s._falseValue))
            : (new(), false);
    }

    public IValue Substitute(IReadOnlyDictionary<IValue, IValue> subs)
    {
        return Create(
            _predicate.Substitute(subs),
            _trueValue.Substitute(subs),
            _falseValue.Substitute(subs));
    }

    public object ToJson()
    {
        return new
        {
            Type = GetType().Name,
            Size = (uint) Size,
            Predicate = _predicate,
            TrueValue = _trueValue,
            FalseValue = _falseValue
        };
    }

    public int GetEquivalencyHash(bool includeSubs)
    {
        return HashCode.Combine(
            _predicate.GetEquivalencyHash(includeSubs),
            _trueValue.GetEquivalencyHash(includeSubs),
            _falseValue.GetEquivalencyHash(includeSubs));
    }
}
