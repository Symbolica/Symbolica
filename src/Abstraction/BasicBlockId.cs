using System;
using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public readonly struct BasicBlockId : IEquatable<BasicBlockId>, IMergeable<IExpression, BasicBlockId>
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

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(BasicBlockId other)
    {
        return (new(), Equals(other));
    }
}
