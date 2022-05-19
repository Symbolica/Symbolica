using System;
using System.Collections.Generic;
using System.Linq;

namespace Symbolica.Expression;

public interface IMergeable<TSub, in T>
{
    (HashSet<(TSub, TSub)> subs, bool) IsEquivalentTo(T other);
    object ToJson();
}

public static class Mergeable
{
    public static (HashSet<(TSub, TSub)> subs, bool) IsEquivalentTo<TSub, K, V>(
        this KeyValuePair<K, V> self,
        KeyValuePair<K, V> other)
        where K : IMergeable<TSub, K>
        where V : IMergeable<TSub, V>
    {
        return self.Key.IsEquivalentTo(other.Key)
            .And(self.Value.IsEquivalentTo(other.Value));
    }

    public static (HashSet<(TSub, TSub)> subs, bool) IsNullableEquivalentTo<TSub, T>(
        T? self,
        T? other)
        where T : struct, IMergeable<TSub, T>
    {
        return self is not null && other is not null
            ? self.Value.IsEquivalentTo(other.Value)
            : (new(), self is null && other is null);
    }

    public static (HashSet<(TSub, TSub)> subs, bool) IsNullableEquivalentTo<TSub, T>(
        T? self,
        T? other)
        where T : class, IMergeable<TSub, T>
    {
        return self is not null && other is not null
            ? self.IsEquivalentTo(other)
            : (new(), self is null && other is null);
    }

    public static (HashSet<(TSub, TSub)> subs, bool) IsSequenceEquivalentTo<TSub, T>(
        this IEnumerable<T> self,
        IEnumerable<T> other,
        Func<T, T, (HashSet<(TSub, TSub)> subs, bool)> isItemEquivalentTo)
    {
        return self
            .Zip(other)
            .Aggregate(
                (new HashSet<(TSub, TSub)>(), true),
                (acc, f) =>
                {
                    if (acc.Item2)
                    {
                        var (subs, equal) = isItemEquivalentTo(f.First, f.Second);
                        subs.UnionWith(acc.Item1);
                        return (subs, equal);
                    }
                    return (acc.Item1, false);
                });
    }

    public static (HashSet<(TSub, TSub)> subs, bool) IsSequenceEquivalentTo<TSub, T>(
        this IEnumerable<T> self,
        IEnumerable<T> other)
        where T : IMergeable<TSub, T>
    {
        return self.IsSequenceEquivalentTo(other, (a, b) => a.IsEquivalentTo(b));
    }

    public static (HashSet<(TSub, TSub)> subs, bool) IsSequenceEquivalentTo<TSub, K, V>(
        this IEnumerable<KeyValuePair<K, V>> self,
        IEnumerable<KeyValuePair<K, V>> other)
        where K : IMergeable<TSub, K>
        where V : IMergeable<TSub, V>
    {
        return self.IsSequenceEquivalentTo(other, (a, b) => a.IsEquivalentTo<TSub, K, V>(b));
    }

    public static (HashSet<(TSub, TSub)> subs, bool) And<TSub>(
        this (HashSet<(TSub, TSub)> subs, bool equivalent) self,
        (HashSet<(TSub, TSub)> subs, bool equivalent) other)
    {
        if (self.equivalent && other.equivalent)
        {
            var subs = new HashSet<(TSub, TSub)>();
            subs.UnionWith(self.subs);
            subs.UnionWith(other.subs);
            return (subs, true);
        }
        return (new(), false);
    }

    public static (HashSet<(RSub, RSub)> subs, bool) MapSubs<TSub, RSub>(
        this (HashSet<(TSub, TSub)> subs, bool equivalent) self,
        Func<TSub, RSub> mapper)
    {
        return (new(self.subs.Select(s => (mapper(s.Item1), mapper(s.Item2)))), self.equivalent);
    }
}
