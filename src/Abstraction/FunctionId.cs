using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public readonly struct FunctionId : IEquatable<FunctionId>, IEquivalent<ExpressionSubs, FunctionId>, IMergeable<FunctionId>
{
    private readonly ulong _value;

    private FunctionId(ulong value)
    {
        _value = value;
    }

    public static explicit operator FunctionId(ulong id)
    {
        return new FunctionId(id);
    }

    public static explicit operator ulong(FunctionId id)
    {
        return id._value;
    }

    public bool Equals(FunctionId other)
    {
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        return obj is FunctionId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static bool operator ==(FunctionId left, FunctionId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(FunctionId left, FunctionId right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return _value.ToString();
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(FunctionId other)
    {
        return (new(), Equals(other));
    }

    public object ToJson()
    {
        return _value;
    }

    public int GetEquivalencyHash()
    {
        return GetHashCode();
    }

    public int GetMergeHash()
    {
        return GetHashCode();
    }

    public bool TryMerge(FunctionId other, IExpression predicate, [MaybeNullWhen(false)] out FunctionId merged)
    {
        merged = this;
        return Equals(other);
    }
}
