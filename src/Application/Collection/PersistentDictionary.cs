using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Symbolica.Collection;

internal sealed class PersistentDictionary<TKey, TValue> : IPersistentDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly ImmutableDictionary<TKey, TValue> _dictionary;

    private PersistentDictionary(ImmutableDictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
    }

    public static IPersistentDictionary<TKey, TValue> Empty =>
        new PersistentDictionary<TKey, TValue>(ImmutableDictionary<TKey, TValue>.Empty);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    public IPersistentDictionary<TKey, TValue> SetItem(TKey key, TValue value)
    {
        return new PersistentDictionary<TKey, TValue>(_dictionary.SetItem(key, value));
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (_dictionary as IEnumerable).GetEnumerator();
    }
}
