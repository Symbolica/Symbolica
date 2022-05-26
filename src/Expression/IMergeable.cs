using System;
using System.Collections.Generic;
using System.Linq;

namespace Symbolica.Expression;

public interface IMergeable<S, in T>
{
    int GetEquivalencyHash(bool includeSubs);
    (HashSet<S> subs, bool) IsEquivalentTo(T other);
    object ToJson();
}

public static class Mergeable
{
    public static (HashSet<S> subs, bool) IsEquivalentTo<S, K, V>(
        this KeyValuePair<K, V> self,
        KeyValuePair<K, V> other)
        where K : IMergeable<S, K>
        where V : IMergeable<S, V>
    {
        return self.Key.IsEquivalentTo(other.Key)
            .And(self.Value.IsEquivalentTo(other.Value));
    }

    public static (HashSet<S> subs, bool) IsNullableEquivalentTo<S, T>(T? self, T? other)
        where T : struct, IMergeable<S, T>
    {
        return self is not null && other is not null
            ? self.Value.IsEquivalentTo(other.Value)
            : (new(), self is null && other is null);
    }

    public static (HashSet<S> subs, bool) IsNullableEquivalentTo<S, T>(T? self, T? other)
        where T : class, IMergeable<S, T>
    {
        return self is not null && other is not null
            ? self.IsEquivalentTo(other)
            : (new(), self is null && other is null);
    }

    public static (HashSet<S> subs, bool) IsSequenceEquivalentTo<S, T>(
        this IEnumerable<T> self,
        IEnumerable<T> other,
        Func<T, T, (HashSet<S> subs, bool)> isItemEquivalentTo)
    {
        return self
            .Zip(other)
            .Aggregate(
                (new HashSet<S>(), true),
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

    public static (HashSet<S> subs, bool) IsSequenceEquivalentTo<S, T>(
        this IEnumerable<T> self,
        IEnumerable<T> other)
        where T : IMergeable<S, T>
    {
        return self.IsSequenceEquivalentTo(other, (a, b) => a.IsEquivalentTo(b));
    }

    public static (HashSet<S> subs, bool) IsSequenceEquivalentTo<S, K, V>(
        this IEnumerable<KeyValuePair<K, V>> self,
        IEnumerable<KeyValuePair<K, V>> other)
        where K : IMergeable<S, K>
        where V : IMergeable<S, V>
    {
        return self.IsSequenceEquivalentTo(other, (a, b) => a.IsEquivalentTo<S, K, V>(b));
    }

    public static (HashSet<S> subs, bool) And<S>(
        this (HashSet<S> subs, bool) self,
        (HashSet<S> subs, bool) other)
    {
        return self.Item2 && other.Item2
            ? (self.subs.Union(other.subs).ToHashSet(), true)
            : (new(), false);
    }

    public static (HashSet<ExpressionSubs> subs, bool) ToHashSet(this (IEnumerable<ExpressionSub> subs, bool equivalent) self)
    {
        return (self.subs.Any() ? new[] { new ExpressionSubs(self.subs) }.ToHashSet() : new(), self.equivalent);
    }
}
