using System.Collections.Generic;

namespace Symbolica.Collection;

public interface IPersistentList<T> : IReadOnlyCollection<T>
{
    IPersistentList<T> Add(T value);
    IPersistentList<T> AddRange(IEnumerable<T> items);
    T Get(int index);
    IPersistentList<T> GetRange(int index, int count);
    IPersistentList<T> SetItem(int index, T value);
    IPersistentList<T> SetItems(IEnumerable<KeyValuePair<int, T>> items);
    IPersistentList<T> SetRange(int index, IPersistentList<T> values);
    int BinarySearch(T item);
}
