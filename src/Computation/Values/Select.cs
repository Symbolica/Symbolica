using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;
using Symbolica.Computation.Values.Constants;
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

    private static IValue CreateUnderPredicate(IValue pathPredicate, IValue predicate, IValue trueValue, IValue falseValue)
    {
        static IValue RecreateUnderPredicate(IValue pathPredicate, Select select)
        {
            bool MustFollowBranch(IValue p, IValue notP)
                => (p is IConstantValue c1 && c1.AsBool())
                    || (notP is IConstantValue c2 && !c2.AsBool())
                    || pathPredicate.Equals(p);

            var and = LogicalAnd.Create(pathPredicate, select._predicate);
            var andNot = LogicalAnd.Create(pathPredicate, LogicalNot.Create(select._predicate));
            return MustFollowBranch(and, andNot)
                ? CreateBranch(pathPredicate, select._trueValue)
                : MustFollowBranch(andNot, and)
                    ? CreateBranch(pathPredicate, select._falseValue)
                    : CreateUnderPredicate(pathPredicate, select._predicate, select._trueValue, select._falseValue);
        }

        static IValue CreateBranch(IValue pathPredicate, IValue value)
        {
            return value switch
            {
                Select s => RecreateUnderPredicate(pathPredicate, s),
                _ => value
            };
        }

        return (predicate,
            CreateBranch(LogicalAnd.Create(pathPredicate, predicate), trueValue),
            CreateBranch(LogicalAnd.Create(pathPredicate, LogicalNot.Create(predicate)), falseValue)) switch
        {
            (IConstantValue p, var t, var f) => p.AsBool() ? t : f,
            (_, Select t, var f) when t._falseValue.Equals(f)
                => CreateUnderPredicate(pathPredicate, LogicalAnd.Create(predicate, t._predicate), t._trueValue, f),
            (_, Select t, var f) when t._trueValue.Equals(f)
                => CreateUnderPredicate(pathPredicate, LogicalAnd.Create(predicate, LogicalNot.Create(t._predicate)), t._falseValue, f),
            (_, var t, Select f) when t is not Select
                => CreateUnderPredicate(pathPredicate, LogicalNot.Create(predicate), f, t),
            (_, var t, var f) when t.Equals(f) => t,
            // These two should be covered by CantBranch clauses now
            (_, Select t, Select f) when t._trueValue.Equals(f._trueValue) && t._falseValue.Equals(f._falseValue)
                => CreateUnderPredicate(
                    pathPredicate,
                    LogicalOr.Create(
                        LogicalAnd.Create(predicate, t._predicate),
                        LogicalAnd.Create(LogicalNot.Create(predicate), f._predicate)),
                    t._trueValue,
                    t._falseValue),
            (_, Select t, Select f) when t._trueValue.Equals(f._falseValue) && t._falseValue.Equals(f._trueValue)
                => CreateUnderPredicate(
                    pathPredicate,
                    LogicalOr.Create(
                        LogicalAnd.Create(predicate, t._predicate),
                        LogicalAnd.Create(LogicalNot.Create(predicate), LogicalNot.Create(f._predicate))),
                    t._trueValue,
                    t._falseValue),
            var (p, t, f) => new Select(p, t, f)
        };
    }

    public static IValue Create(IValue predicate, IValue trueValue, IValue falseValue)
    {
        return CreateUnderPredicate(new ConstantBool(true), predicate, trueValue, falseValue);
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
