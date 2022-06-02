using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal sealed class PersistentAllocations : IPersistentAllocations
{
    private readonly ICollectionFactory _collectionFactory;
    private readonly IPersistentStack<IExpression> _allocations;

    private PersistentAllocations(
        ICollectionFactory collectionFactory,
        IPersistentStack<IExpression> allocations)
    {
        _collectionFactory = collectionFactory;
        _allocations = allocations;
    }

    public IPersistentAllocations Add(IExpression allocation)
    {
        return new PersistentAllocations(_collectionFactory, _allocations.Push(allocation));
    }

    public IEnumerator<IExpression> GetEnumerator()
    {
        return _allocations.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static IPersistentAllocations Create(ICollectionFactory collectionFactory)
    {
        return new PersistentAllocations(collectionFactory, collectionFactory.CreatePersistentStack<IExpression>());
    }

    public int GetEquivalencyHash()
    {
        var hash = new HashCode();
        foreach (var allocation in _allocations)
            hash.Add(allocation.GetEquivalencyHash());
        return hash.ToHashCode();
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(IPersistentAllocations other)
    {
        return other is PersistentAllocations pa
            ? _allocations.IsSequenceEquivalentTo(
                pa._allocations,
                (x, y) => x.IsEquivalentTo(y).ToHashSet())
            : (new(), false);
    }

    public object ToJson()
    {
        return _allocations.Select(a => a.ToJson()).ToArray();
    }

    public int GetMergeHash()
    {
        var hash = new HashCode();
        foreach (var allocation in _allocations)
            hash.Add(allocation.GetMergeHash());
        return hash.ToHashCode();
    }

    public bool TryMerge(IPersistentAllocations other, IExpression predicate, [MaybeNullWhen(false)] out IPersistentAllocations merged)
    {
        if (other is PersistentAllocations pa && _allocations.TryMerge(pa._allocations, predicate, out var mergedAllocations))
        {
            merged = new PersistentAllocations(
                _collectionFactory,
                _collectionFactory.CreatePersistentStack(mergedAllocations));
            return true;
        }
        merged = null;
        return false;
    }
}
