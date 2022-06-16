using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Symbolica.Collection;
using Symbolica.Computation.Values;
using Symbolica.Computation.Values.Constants;
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

    public object ToJson()
    {
        return _assertions.Select(a => a.ToJson()).ToArray();
    }

    public bool TryMerge(ISpace space, out (ISpace Merged, IExpression Predicate) result)
    {
        // Create the union of the two spaces
        // Build the predicate by determining that the assertions that are a subset of the current space (only consider those that aren't structurally equivalent to the ones in the union)
        // The predicate is then the LogicalAnd of one sides disjoint assertions
        // If there is an assertion on one side that is not disjoint from the other, but is a subspace of the union then fail the merge
        result = default;
        return space is PersistentSpace ps && TryMerge(ps, out result);
    }

    public bool TryMerge(PersistentSpace other, out (ISpace Merged, IExpression Predicate) result)
    {
        // var unmerged = new List<IValue>();
        // var others = other._assertions.Reverse().ToList();
        // var mergedAssertions = _collectionFactory.CreatePersistentStack<IValue>();
        // foreach (var assertion in _assertions.Reverse())
        // {
        //     var isAssertionMerged = false;
        //     foreach (var otherAssertion in others)
        //         if (assertion.TryMerge(otherAssertion, out var mergedAssertion))
        //         {
        //             others.Remove(otherAssertion);
        //             isAssertionMerged = true;
        //             if (mergedAssertion is not null)
        //                 mergedAssertions = mergedAssertions.Push(mergedAssertion);
        //             break;
        //         }
        //     if (!isAssertionMerged)
        //         unmerged.Add(assertion);
        // }

        IValue Conjunction(IEnumerable<IValue> assertions)
        {
            return assertions.Aggregate(new ConstantBool(true) as IValue, LogicalAnd.Create);
        }

        // Now unmerged and others are the difference between the two spaces
        // Add one set to the merged lot and then try and prove that the other set is disjoint
        // i.e. there are no value in that sub space that are in the combined space with the other
        // If not disjoint then check whether either side is a subset of the other
        bool IsDisjoint(
            IPersistentStack<IValue> intersection,
            IEnumerable<IValue> leftDiff,
            IEnumerable<IValue> rightDiff)
        {
            var space = new PersistentSpace(_collectionFactory, intersection.PushMany(leftDiff));
            var otherAssertions = Conjunction(intersection.Concat(rightDiff));
            using var solver = space.CreateSolver();
            return !solver.IsSatisfiable(otherAssertions);
        }

        bool IsSubSpace(
            IPersistentStack<IValue> intersection,
            IEnumerable<IValue> leftDiff,
            IEnumerable<IValue> rightDiff)
        {
            var space = new PersistentSpace(_collectionFactory, intersection.PushMany(rightDiff));
            var otherAssertions = Conjunction(intersection.Concat(leftDiff));
            using var solver = space.CreateSolver();
            return !solver.IsSatisfiable(LogicalNot.Create(otherAssertions));
        }

        var intersection = _collectionFactory.CreatePersistentStack(_assertions.Intersect(other._assertions));
        var leftDiff = _assertions.Except(intersection);
        var rightDiff = other._assertions.Except(intersection);

        if (!leftDiff.Any() && !rightDiff.Any())
            throw new Exception("Seems like these spaces are identical. Check for state equality before merging?");

        if (IsSubSpace(intersection, leftDiff, rightDiff))
        {
            var mergedSpace = new PersistentSpace(
                _collectionFactory,
                intersection.PushMany(leftDiff));
            result = (mergedSpace,
                new Expression(
                    _collectionFactory,
                    Conjunction(rightDiff.Select(LogicalNot.Create))));
            return true;
        }

        if (IsSubSpace(intersection, rightDiff, leftDiff))
        {
            var mergedSpace = new PersistentSpace(
                _collectionFactory,
                intersection.PushMany(rightDiff));
            result = (mergedSpace, new Expression(_collectionFactory, Conjunction(leftDiff)));
            return true;
        }

        if (IsDisjoint(intersection, leftDiff, rightDiff))
        {
            var disjunction = LogicalOr.Create(Conjunction(leftDiff), Conjunction(rightDiff));
            var mergedSpace = new PersistentSpace(
                _collectionFactory,
                IsSubSpace(intersection, new[] { disjunction }, Enumerable.Empty<IValue>())
                    ? intersection
                    : intersection.Push(disjunction));
            result = (mergedSpace, new Expression(_collectionFactory, Conjunction(leftDiff)));
            return true;
        }

        result = default;
        return false;
    }
}
