using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Symbolica.Collection;
using Symbolica.Computation.Values;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal sealed class PersistentSpace : IPersistentSpace
{
    private readonly IPersistentStack<IValue> _assertions;
    private readonly ICollectionFactory _collectionFactory;

    private PersistentSpace(ICollectionFactory collectionFactory, IPersistentStack<IValue> assertions)
    {
        _collectionFactory = collectionFactory;
        _assertions = assertions;
    }

    public IPersistentSpace Assert(IValue assertion)
    {
        IPersistentStack<IValue> Assertions()
        {
            var (merged, assertions) = _assertions.Reverse().Aggregate(
                (merged: false, assertions: _collectionFactory.CreatePersistentStack<IValue>()),
                (x, a) =>
                    !x.merged && a.TryMerge(assertion, out var mergedAssertion)
                        ? (true, mergedAssertion is null
                            ? x.assertions
                            : x.assertions.Push(mergedAssertion))
                        : (x.merged, x.assertions.Push(a)));
            return merged
                ? assertions
                : assertions.Push(assertion);
        }
        return assertion is IConstantValue
            ? this
            : new PersistentSpace(_collectionFactory, Assertions());
    }

    public ISolver CreateSolver()
    {
        var solver = new LazySolver();
        solver.Assert(_assertions);

        return solver;
    }

    public IExample GetExample()
    {
        using var solver = CreateSolver();

        return solver.GetExample();
    }

    public static ISpace Create(ICollectionFactory collectionFactory)
    {
        return new PersistentSpace(collectionFactory, collectionFactory.CreatePersistentStack<IValue>());
    }

    public bool Equals(ISpace? other)
    {
        return Equals(other as PersistentSpace);
    }

    public bool Equals(PersistentSpace? other)
    {
        return other is not null && _assertions.SequenceEqual(other._assertions);
    }

    public ISpace Substitute(IReadOnlyDictionary<IExpression, IExpression> subs)
    {
        var assertions = _assertions
            .Reverse()
            .Select(a =>
                a.Substitute(
                    subs.ToDictionary(
                        x => x.Key.As<Expression>().Value,
                        x => x.Value.As<Expression>().Value)))
            .ToHashSet()
            .Aggregate(
                _collectionFactory.CreatePersistentStack<IValue>(),
                (acc, a) => acc.Push(a));
        return new PersistentSpace(_collectionFactory, assertions);
    }

    public bool SubsAreEquivalent(IEnumerable<ExpressionSub> subs, ISpace other)
    {
        return other is PersistentSpace ps && SubsAreEquivalent(
            subs.Select(s => (s.New.As<Expression>().Value, s.Old.As<Expression>().Value)).ToHashSet(),
            ps);
    }

    private IEnumerable<IValue> DependentSymbols(IValue symbol, ISet<IValue> ignoring)
    {
        return new[] { symbol }.Concat(
            ignoring.Add(symbol)
                ? _assertions
                    .Where(a => a.Symbols.Contains(symbol))
                    .SelectMany(a => a.Symbols)
                    .SelectMany(s => DependentSymbols(s, ignoring))
                : Enumerable.Empty<IValue>());
    }

    private bool SubsAreEquivalent(HashSet<(IValue, IValue)> subs, PersistentSpace other)
    {
        static string RenameOldSymbol(object x)
        {
            return $"{x}-old";
        }

        if (!subs.Any())
            return true;

        var subsEqual = subs
            .Select(s => Equal.Create(s.Item1, s.Item2.RenameSymbols(RenameOldSymbol)))
            .Aggregate(LogicalAnd.Create);

        var isStillRelevant = subs.SelectMany(s => other.DependentSymbols(s.Item2, new HashSet<IValue>())).ToHashSet();

        var previousAssertions = other._assertions
            .Where(a => a.Symbols.Any(isStillRelevant.Contains))
            .Aggregate(LogicalAnd.Create)
            .RenameSymbols(RenameOldSymbol);

        var isNotSubSpace = LogicalAnd.Create(subsEqual, Not.Create(previousAssertions));

        using var solver = CreateSolver();
        return !solver.IsSatisfiable(isNotSubSpace);
    }

    public bool TryMerge(ISpace other, [MaybeNullWhen(false)] out ISpace merged)
    {
        merged = null;
        return other is PersistentSpace ps && TryMerge(ps, out merged);
    }

    private bool TryMerge(PersistentSpace other, [MaybeNullWhen(false)] out ISpace merged)
    {
        // Foreach assertion in other try and merge with one in this space and then add those to the merged space
        // The merge is successful if the one side has no unmerged assertions left (i.e. it is the same or a subset of the other)
        var unmerged = new List<IValue>();
        var others = other._assertions.Reverse().ToList();
        var mergedAssertions = _collectionFactory.CreatePersistentStack<IValue>();
        foreach (var assertion in _assertions.Reverse())
        {
            var isAssertionMerged = false;
            foreach (var otherAssertion in others)
                if (assertion.TryMerge(otherAssertion, out var mergedAssertion))
                {
                    others.Remove(otherAssertion);
                    isAssertionMerged = true;
                    if (mergedAssertion is not null)
                        mergedAssertions = mergedAssertions.Push(mergedAssertion);
                    break;
                }
            if (!isAssertionMerged)
                unmerged.Add(assertion);
        }
        // If there are common symbols left on both sides then we'll fail the merge as it means there
        // are constraints for that symbol that should be merged and retained
        // This could be made better by first trying to improve the merging above to cover more scenarios
        // or falling back to just doing a disjunction here of the assertions that appear on both sides for
        // a given symbol.
        if (unmerged.SelectMany(a => a.Symbols).Intersect(others.SelectMany(a => a.Symbols)).Any())
        {
            merged = null;
            return false;
        }
        merged = new PersistentSpace(_collectionFactory, mergedAssertions);
        return true;
    }

    public object ToJson()
    {
        return _assertions.Select(a => a.ToJson()).ToArray();
    }
}
