namespace Symbolica.Collection
{
    internal sealed class CollectionFactory : ICollectionFactory
    {
        public IPersistentStack<T> CreatePersistentStack<T>()
        {
            return PersistentStack<T>.Empty;
        }

        public IPersistentList<T> CreatePersistentList<T>()
        {
            return PersistentList<T>.Empty;
        }

        public IPersistentDictionary<TKey, TValue> CreatePersistentDictionary<TKey, TValue>()
            where TKey : notnull
        {
            return PersistentDictionary<TKey, TValue>.Empty;
        }
    }
}
