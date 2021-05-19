namespace Symbolica.Collection
{
    public interface ICollectionFactory
    {
        IPersistentStack<T> CreatePersistentStack<T>();

        IPersistentList<T> CreatePersistentList<T>();

        IPersistentDictionary<TKey, TValue> CreatePersistentDictionary<TKey, TValue>()
            where TKey : notnull;
    }
}