using System.Diagnostics.CodeAnalysis;

namespace Symbolica.Collection
{
    public interface IPersistentDictionary<in TKey, TValue>
        where TKey : notnull
    {
        bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
        IPersistentDictionary<TKey, TValue> SetItem(TKey key, TValue value);
    }
}
