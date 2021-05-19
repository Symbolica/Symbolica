using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Symbolica.Collection;

namespace Symbolica.Application.Collection
{
    internal sealed class PersistentList<T> : IPersistentList<T>
    {
        private readonly ImmutableList<T> _list;

        private PersistentList(ImmutableList<T> list)
        {
            _list = list;
        }

        public static IPersistentList<T> Empty =>
            new PersistentList<T>(ImmutableList<T>.Empty);

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _list.Count;

        public IPersistentList<T> Add(T value)
        {
            return new PersistentList<T>(_list.Add(value));
        }

        public IPersistentList<T> AddRange(IEnumerable<T> items)
        {
            return new PersistentList<T>(_list.AddRange(items));
        }

        public T Get(int index)
        {
            return _list[index];
        }

        public IPersistentList<T> GetRange(int index, int count)
        {
            return new PersistentList<T>(_list.GetRange(index, count));
        }

        public IPersistentList<T> SetItem(int index, T value)
        {
            return new PersistentList<T>(_list.SetItem(index, value));
        }

        public IPersistentList<T> SetItems(IEnumerable<KeyValuePair<int, T>> items)
        {
            var builder = _list.ToBuilder();

            foreach (var (key, value) in items)
                builder[key] = value;

            return new PersistentList<T>(builder.ToImmutable());
        }

        public IPersistentList<T> SetRange(int index, IPersistentList<T> values)
        {
            return new PersistentList<T>(_list.RemoveRange(index, values.Count).InsertRange(index, values));
        }

        public int BinarySearch(T item)
        {
            return _list.BinarySearch(item);
        }
    }
}