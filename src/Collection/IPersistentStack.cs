using System.Collections.Generic;

namespace Symbolica.Collection;

public interface IPersistentStack<T> : IEnumerable<T>
{
    IPersistentStack<T> Push(T value);
    IPersistentStack<T> Pop(out T value);
}
