using System;
using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public readonly struct InstructionId : IEquatable<InstructionId>, IMergeable<IExpression, InstructionId>
{
    private readonly ulong _value;

    private InstructionId(ulong value)
    {
        _value = value;
    }

    public static explicit operator InstructionId(ulong id)
    {
        return new InstructionId(id);
    }

    public static explicit operator ulong(InstructionId id)
    {
        return id._value;
    }

    public bool Equals(InstructionId other)
    {
        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        return obj is InstructionId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    public static bool operator ==(InstructionId left, InstructionId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(InstructionId left, InstructionId right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return _value.ToString();
    }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(InstructionId other)
    {
        return (new(), Equals(other));
    }

    public object ToJson()
    {
        return _value;
    }

    public int GetEquivalencyHash(bool includeSubs)
    {
        return GetHashCode();
    }
}
