using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Symbolica.Expression;

public sealed record class ExpressionSub(IExpression New, IExpression Old);

public sealed class ExpressionSubs : IEquatable<ExpressionSubs>, IReadOnlySet<ExpressionSub>
{
    private readonly HashSet<ExpressionSub> _subs;

    public ExpressionSubs(HashSet<ExpressionSub> subs)
    {
        _subs = subs;
    }

    public ExpressionSubs(IEnumerable<ExpressionSub> subs)
        : this(subs.ToHashSet())
    { }

    public int Count => _subs.Count;

    public override bool Equals(object? other)
    {
        return Equals(other as ExpressionSubs);
    }

    public bool Equals(ExpressionSubs? other)
    {
        return other is not null && SetEquals(other);
    }

    public override int GetHashCode()
    {
        var subsHash = new HashCode();
        foreach (var sub in _subs)
            subsHash.Add(sub);
        return subsHash.ToHashCode();
    }

    public bool Contains(ExpressionSub item)
    {
        return _subs.Contains(item);
    }

    public bool IsProperSubsetOf(IEnumerable<ExpressionSub> other)
    {
        return _subs.IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<ExpressionSub> other)
    {
        return _subs.IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<ExpressionSub> other)
    {
        return _subs.IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<ExpressionSub> other)
    {
        return _subs.IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<ExpressionSub> other)
    {
        return _subs.Overlaps(other);
    }

    public bool SetEquals(IEnumerable<ExpressionSub> other)
    {
        return _subs.SetEquals(other);
    }

    public IEnumerator<ExpressionSub> GetEnumerator()
    {
        return _subs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
