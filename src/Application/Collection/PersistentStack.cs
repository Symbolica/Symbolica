using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Symbolica.Collection
{
    internal sealed class PersistentStack<T> : IPersistentStack<T>
    {
        private readonly ImmutableStack<T> _stack;

        private PersistentStack(ImmutableStack<T> stack)
        {
            _stack = stack;
        }

        public static IPersistentStack<T> Empty =>
            new PersistentStack<T>(ImmutableStack<T>.Empty);

        public IEnumerator<T> GetEnumerator()
        {
            return _stack.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IPersistentStack<T> Push(T value)
        {
            return new PersistentStack<T>(_stack.Push(value));
        }

        public IPersistentStack<T> Pop(out T value)
        {
            return new PersistentStack<T>(_stack.Pop(out value));
        }
    }
}
