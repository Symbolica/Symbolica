using System.Collections;
using System.Collections.Generic;
using Symbolica.Collection;
using Symbolica.Expression.Values;

namespace Symbolica.Implementation.Stack;

internal sealed class PersistentAllocations : IPersistentAllocations
{
    private readonly IPersistentStack<Address> _allocations;

    private PersistentAllocations(IPersistentStack<Address> allocations)
    {
        _allocations = allocations;
    }

    public IPersistentAllocations Add(Address allocation)
    {
        return new PersistentAllocations(_allocations.Push(allocation));
    }

    public IEnumerator<Address> GetEnumerator()
    {
        return _allocations.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static IPersistentAllocations Create(ICollectionFactory collectionFactory)
    {
        return new PersistentAllocations(collectionFactory.CreatePersistentStack<Address>());
    }
}
