using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public readonly struct BasicBlockId : IEquatable<BasicBlockId>, IEquivalent<ExpressionSubs, BasicBlockId>, IMergeable<BasicBlockId>
{
    private readonly ulong _value;

    private BasicBlockId(ulong value)
    {
        _value = value;
    }

    public static explicit operator BasicBlockId(ulong id)
    {
        return new BasicBlockId(id);
    }

    public static explicit operator ulong(BasicBlockId id)
    {
        return id._value;
    }

    public bool Equals(BasicBlockId other)
    {
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        return obj is BasicBlockId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static bool operator ==(BasicBlockId left, BasicBlockId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BasicBlockId left, BasicBlockId right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return _value.ToString();
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(BasicBlockId other)
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

    public bool TryMerge(BasicBlockId other, IExpression predicate, [MaybeNullWhen(false)] out BasicBlockId merged)
    {
        merged = this;
        return Equals(other);
    }
}
