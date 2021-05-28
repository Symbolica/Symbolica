using System.Collections;
using System.Collections.Generic;
using Symbolica.Collection;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack
{
    internal sealed class PersistentAllocations : IPersistentAllocations
    {
        private readonly IPersistentStack<IExpression> _allocations;

        private PersistentAllocations(IPersistentStack<IExpression> allocations)
        {
            _allocations = allocations;
        }

        public IPersistentAllocations Add(IExpression allocation)
        {
            return new PersistentAllocations(_allocations.Push(allocation));
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
            return new PersistentAllocations(collectionFactory.CreatePersistentStack<IExpression>());
        }
    }
}
