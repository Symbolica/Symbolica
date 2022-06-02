using System.Collections.Generic;
using System.Linq;

namespace Symbolica.Collection;

public interface ICollectionFactory
{
    IPersistentStack<T> CreatePersistentStack<T>();

    IPersistentList<T> CreatePersistentList<T>();

    IPersistentDictionary<TKey, TValue> CreatePersistentDictionary<TKey, TValue>()
        where TKey : notnull;
}

public static class CollectionExtensions
{
    public static IPersistentList<T> CreatePersistentList<T>(this ICollectionFactory self, IEnumerable<T> items)
    {
        return self.CreatePersistentList<T>().AddRange(items);
    }

    public static IPersistentStack<T> CreatePersistentStack<T>(this ICollectionFactory self, IEnumerable<T> items)
    {
        return self.CreatePersistentStack<T>().PushMany(items);
    }

    public static IPersistentStack<T> PushMany<T>(this IPersistentStack<T> self, IEnumerable<T> items)
    {
        return items.Reverse().Aggregate(self, (acc, x) => acc.Push(x));
    }

    public static IPersistentDictionary<K, V> CreatePersistentDictionary<K, V>(
        this ICollectionFactory self,
        IEnumerable<KeyValuePair<K, V>> items)
        where K : notnull
    {
        return self.CreatePersistentDictionary<K, V>().SetMany(items);
    }

    public static IPersistentDictionary<K, V> SetMany<K, V>(
        this IPersistentDictionary<K, V> self,
        IEnumerable<KeyValuePair<K, V>> items)
        where K : notnull
    {
        return items.Aggregate(self, (acc, x) => acc.SetItem(x.Key, x.Value));
    }
}
