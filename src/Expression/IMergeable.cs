using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Symbolica.Expression;

public interface IEquivalent<S, in T>
{
    int GetEquivalencyHash();
    (HashSet<S> subs, bool) IsEquivalentTo(T other);
    object ToJson();
}

public interface IMergeable<T>
{
    int GetMergeHash();
    bool TryMerge(T other, IExpression predicate, [MaybeNullWhen(false)] out T merged);
}

public static class Mergeable
{
    public delegate bool Merger<T>(T self, T other, IExpression predicate, [MaybeNullWhen(false)] out T merged);

    public static bool TryMerge<T>(
        this IEnumerable<T> self,
        IEnumerable<T> others,
        IExpression predicate,
        Merger<T> merger,
        [MaybeNullWhen(false)] out IEnumerable<T> merged)
    {
        merged = null;
        var mergedList = new List<T>();
        if (self.Count() != others.Count())
            return false;

        foreach (var (x, y) in self.Zip(others))
            if (merger(x, y, predicate, out var z))
                mergedList.Add(z);
            else
                return false;

        merged = mergedList;
        return true;
    }

    public static bool TryMerge<T>(
        this IEnumerable<T> self,
        IEnumerable<T> others,
        IExpression predicate,
        [MaybeNullWhen(false)] out IEnumerable<T> merged)
        where T : IMergeable<T>
    {
        return self.TryMerge(
            others,
            predicate,
            (T x, T y, IExpression predicate, [MaybeNullWhen(false)] out T mergedItem) => x.TryMerge(y, predicate, out mergedItem),
            out merged);
    }

    public static bool TryMerge<K, V>(
        this IEnumerable<KeyValuePair<K, V>> self,
        IEnumerable<KeyValuePair<K, V>> others,
        IExpression predicate,
        [MaybeNullWhen(false)] out IEnumerable<KeyValuePair<K, V>> merged)
        where K : IMergeable<K>
        where V : IMergeable<V>
    {
        return self.TryMerge(
            others,
            predicate,
            (KeyValuePair<K, V> x, KeyValuePair<K, V> y, IExpression predicate, out KeyValuePair<K, V> mergedItem) =>
            {
                if (x.Key.TryMerge(y.Key, predicate, out var mergedKey)
                    && x.Value.TryMerge(y.Value, predicate, out var mergedValue))
                {
                    mergedItem = new(mergedKey, mergedValue);
                    return true;
                }
                mergedItem = new();
                return false;
            },
            out merged);
    }

    public static bool TryMergeNullable<T>(T? self, T? other, IExpression predicate, out T? merged)
        where T : class, IMergeable<T>
    {
        merged = null;
        return self is not null && other is not null && self.TryMerge(other, predicate, out merged)
            || self is null && other is null;
    }

    public static bool TryMergeNullable<T>(T? self, T? other, IExpression predicate, out T merged)
        where T : struct, IMergeable<T>
    {
        merged = new();
        return self is not null && other is not null && self.Value.TryMerge(other.Value, predicate, out merged)
            || self is null && other is null;
    }
}

public static class Equivalent
{
    public static (HashSet<S> subs, bool) IsEquivalentTo<S, K, V>(
        this KeyValuePair<K, V> self,
        KeyValuePair<K, V> other)
        where K : IEquivalent<S, K>
        where V : IEquivalent<S, V>
    {
        return self.Key.IsEquivalentTo(other.Key)
            .And(self.Value.IsEquivalentTo(other.Value));
    }

    public static (HashSet<S> subs, bool) IsNullableEquivalentTo<S, T>(T? self, T? other)
        where T : struct, IEquivalent<S, T>
    {
        return self is not null && other is not null
            ? self.Value.IsEquivalentTo(other.Value)
            : (new(), self is null && other is null);
    }

    public static (HashSet<S> subs, bool) IsNullableEquivalentTo<S, T>(T? self, T? other)
        where T : class, IEquivalent<S, T>
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
        where T : IEquivalent<S, T>
    {
        return self.IsSequenceEquivalentTo(other, (a, b) => a.IsEquivalentTo(b));
    }

    public static (HashSet<S> subs, bool) IsSequenceEquivalentTo<S, K, V>(
        this IEnumerable<KeyValuePair<K, V>> self,
        IEnumerable<KeyValuePair<K, V>> other)
        where K : IEquivalent<S, K>
        where V : IEquivalent<S, V>
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
