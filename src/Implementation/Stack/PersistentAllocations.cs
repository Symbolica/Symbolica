using System.Collections;
using System.Collections.Generic;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal sealed class PersistentAllocations : IPersistentAllocations
{
    private readonly IPersistentStack<IExpression<IType>> _allocations;

    private PersistentAllocations(IPersistentStack<IExpression<IType>> allocations)
    {
        _allocations = allocations;
    }

    public IPersistentAllocations Add(IExpression<IType> allocation)
    {
        return new PersistentAllocations(_allocations.Push(allocation));
    }

    public IEnumerator<IExpression<IType>> GetEnumerator()
    {
        return _allocations.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static IPersistentAllocations Create(ICollectionFactory collectionFactory)
    {
        return new PersistentAllocations(collectionFactory.CreatePersistentStack<IExpression<IType>>());
    }
}
