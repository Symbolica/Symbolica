using System.Collections.Generic;
using System.Linq;

namespace Symbolica.Deserialization
{
    internal abstract record Serializable<T>
    {
        public abstract T Convert();
    }

    internal static class SerializableEnumerableExtensions
    {
        public static T[] Convert<T>(this IEnumerable<Serializable<T>> self)
        {
            return self.Select(x => x.Convert()).ToArray();
        }
    }
}