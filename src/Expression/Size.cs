using System;
using System.Numerics;

namespace Symbolica.Expression;

public readonly struct Size : IEquatable<Size>, IComparable<Size>
{
    private Size(uint bits)
    {
        Bits = bits;
    }

    public static Size FromBits(BigInteger bits)
    {
        return new Size((uint) bits);
    }

    public static Size FromBytes(BigInteger bytes)
    {
        return FromBits(Byte.Bits * (uint) bytes);
    }

    public static Size Zero => FromBits(0U);
    public static Size Bit => FromBits(1U);
    public static Size Byte => FromBits(8U);

    public uint Bits { get; }
    public uint Bytes => (Byte.Bits - Bit.Bits + Bits) / Byte.Bits;

    public Size AlignToBytes(Size alignment)
    {
        return FromBytes(alignment.Bytes * ((Bytes + alignment.Bytes - 1U) / alignment.Bytes));
    }

    public static Size operator +(Size left, Size right)
    {
        return FromBits(left.Bits + right.Bits);
    }

    public static Size operator -(Size left, Size right)
    {
        return FromBits(left.Bits - right.Bits);
    }

    public static Size operator *(Size left, uint right)
    {
        return FromBits(left.Bits * right);
    }

    public bool Equals(Size other)
    {
        return Bits == other.Bits;
    }

    public override bool Equals(object? obj)
    {
        return obj is Size other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Bits.GetHashCode();
    }

    public static bool operator ==(Size left, Size right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Size left, Size right)
    {
        return !left.Equals(right);
    }

    public int CompareTo(Size other)
    {
        return Bits.CompareTo(other.Bits);
    }

    public static bool operator <(Size left, Size right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(Size left, Size right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(Size left, Size right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(Size left, Size right)
    {
        return left.CompareTo(right) >= 0;
    }

    public override string ToString()
    {
        return Bits.ToString();
    }
}
