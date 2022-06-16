using System;
using System.Collections.Generic;
using System.Linq;
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

    public ISet<IValue> Symbols => _predicate.Symbols.Union(_trueValue.Symbols).Union(_falseValue.Symbols).ToHashSet();

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
            // (_, _, IConstantValue f) when f.AsUnsigned().IsZero
            //     => And.Create(SignExtend.Create(trueValue.Size, predicate), trueValue),
            // (_, IConstantValue t, _) when t.AsUnsigned().IsZero
            //     => And.Create(SignExtend.Create(falseValue.Size, Not.Create(predicate)), trueValue),
            (_, Select t, _) when predicate.Equals(t._predicate)
                => Create(predicate, t._trueValue, falseValue),
            (_, Select t, _) when predicate.Equals(LogicalNot.Create(t._predicate))
                => Create(predicate, t._falseValue, falseValue),
            (_, _, Select f) when predicate.Equals(f._predicate)
                => Create(predicate, trueValue, f._falseValue),
            (_, _, Select f) when predicate.Equals(LogicalNot.Create(f._predicate))
                => Create(predicate, trueValue, f._trueValue),
            (_, Select t, _) when t._falseValue.Equals(falseValue)
                => Create(LogicalAnd.Create(predicate, t._predicate), t._trueValue, falseValue),
            (_, Select t, _) when t._trueValue.Equals(falseValue)
                => Create(LogicalAnd.Create(predicate, LogicalNot.Create(t._predicate)), t._falseValue, falseValue),
            (_, _, Select _) when trueValue is not Select
                => Create(LogicalNot.Create(predicate), falseValue, trueValue),
            _ when trueValue.Equals(falseValue) => trueValue,
            (_, Select t, Select f) when t._trueValue.Equals(f._trueValue) && t._falseValue.Equals(f._falseValue)
                => Create(
                    LogicalOr.Create(
                        LogicalAnd.Create(predicate, t._predicate),
                        LogicalAnd.Create(LogicalNot.Create(predicate), f._predicate)),
                    t._trueValue,
                    t._falseValue),
            (_, Select t, Select f) when t._trueValue.Equals(f._falseValue) && t._falseValue.Equals(f._trueValue)
                => Create(
                    LogicalOr.Create(
                        LogicalAnd.Create(predicate, t._predicate),
                        LogicalAnd.Create(LogicalNot.Create(predicate), LogicalNot.Create(f._predicate))),
                    t._trueValue,
                    t._falseValue),
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
            Predicate = _predicate.ToJson(),
            TrueValue = _trueValue.ToJson(),
            FalseValue = _falseValue.ToJson()
        };
    }

    public int GetEquivalencyHash()
    {
        return HashCode.Combine(
            _predicate.GetEquivalencyHash(),
            _trueValue.GetEquivalencyHash(),
            _falseValue.GetEquivalencyHash());
    }

    public IValue RenameSymbols(Func<string, string> renamer)
    {
        return Create(
            _predicate.RenameSymbols(renamer),
            _trueValue.RenameSymbols(renamer),
            _falseValue.RenameSymbols(renamer));
    }
}
