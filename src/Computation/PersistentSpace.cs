using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Symbolica.Collection;
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

    public bool TryMerge(ISpace other, [MaybeNullWhen(false)] out ISpace merged)
    {
        merged = null;
        return other is PersistentSpace ps && TryMerge(ps, out merged);
    }

    private bool TryMerge(PersistentSpace other, [MaybeNullWhen(false)] out ISpace merged)
    {
        var (mergeFailed, mergedAssertions) = other._assertions.Reverse().Aggregate(
            (mergeFailed: false, assertions: _collectionFactory.CreatePersistentStack<IValue>()),
            (x, a) =>
                !x.mergeFailed && TryMerge(a, out var mergedAssertion)
                    ? (false, mergedAssertion is null
                        ? x.assertions
                        : x.assertions.Push(mergedAssertion))
                    : (true, x.assertions));

        merged = mergeFailed
            ? null
            : new PersistentSpace(_collectionFactory, mergedAssertions);
        return !mergeFailed;
    }

    private bool TryMerge(IValue assertion, out IValue? merged)
    {
        foreach (var other in _assertions)
        {
            if (other.TryMerge(assertion, out merged))
                return true;
        }

        merged = null;
        return false;
    }

    public object ToJson()
    {
        return _assertions.Select(a => a.ToJson()).ToArray();
    }
}
