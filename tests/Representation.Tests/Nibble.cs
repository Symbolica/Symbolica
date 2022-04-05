using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Expression;

namespace Symbolica.Representation;

internal readonly struct Nibble : IEquatable<Nibble>
{
    private Nibble(bool b3, bool b2, bool b1, bool b0)
    {
        B3 = b3;
        B2 = b2;
        B1 = b1;
        B0 = b0;
    }

    public bool B3 { get; }
    public bool B2 { get; }
    public bool B1 { get; }
    public bool B0 { get; }

    public static Size Size => Size.FromBits(4U);
    public static Nibble Zero => new(false, false, false, false);
    public static Nibble One => new(false, false, false, true);

    public static explicit operator Nibble(BigInteger nibble)
    {
        return new Nibble(
            (nibble & 0b1000) == 0b1000,
            (nibble & 0b0100) == 0b0100,
            (nibble & 0b0010) == 0b0010,
            (nibble & 0b0001) == 0b0001);
    }

    public static explicit operator BigInteger(Nibble nibble)
    {
        return new BigInteger(
            (nibble.B3 ? 0b1000 : 0) |
            (nibble.B2 ? 0b0100 : 0) |
            (nibble.B1 ? 0b0010 : 0) |
            (nibble.B0 ? 0b0001 : 0));
    }

    public static IEnumerable<Nibble> GenerateAll()
    {
        return
            from b3 in new[] { false, true }
            from b2 in new[] { false, true }
            from b1 in new[] { false, true }
            from b0 in new[] { false, true }
            select new Nibble(b3, b2, b1, b0);
    }

    public Nibble ArithmeticShiftRight(Nibble shift)
    {
        return shift == Zero
            ? this
            : new Nibble(B3, B3, B2, B1).ArithmeticShiftRight(--shift);
    }

    public Nibble LogicalShiftRight(Nibble shift)
    {
        return shift == Zero
            ? this
            : new Nibble(false, B3, B2, B1).LogicalShiftRight(--shift);
    }

    public Nibble ShiftLeft(Nibble shift)
    {
        return shift == Zero
            ? this
            : new Nibble(B2, B1, B0, false).ShiftLeft(--shift);
    }

    public Nibble UnsignedRemainder(Nibble divisor)
    {
        var remainder = this - divisor;

        return remainder.B3
            ? this
            : remainder.UnsignedRemainder(divisor);
    }

    public static Nibble operator -(Nibble value)
    {
        return new Nibble(!value.B3, !value.B2, !value.B1, !value.B0) + One;
    }

    public static Nibble operator -(Nibble left, Nibble right)
    {
        return right == Zero
            ? left
            : --left + --right;
    }

    public static Nibble operator +(Nibble left, Nibble right)
    {
        return right == Zero
            ? left
            : ++left + --right;
    }

    public static Nibble operator --(Nibble value)
    {
        return new Nibble(
            !value.B3 ^ (value.B2 | value.B1 | value.B0),
            !value.B2 ^ (value.B1 | value.B0),
            !value.B1 ^ value.B0,
            !value.B0);
    }

    public static Nibble operator ++(Nibble value)
    {
        return new Nibble(
            value.B3 ^ (value.B2 & value.B1 & value.B0),
            value.B2 ^ (value.B1 & value.B0),
            value.B1 ^ value.B0,
            !value.B0);
    }

    public static Nibble operator &(Nibble left, Nibble right)
    {
        return new Nibble(
            left.B3 && right.B3,
            left.B2 && right.B2,
            left.B1 && right.B1,
            left.B0 && right.B0);
    }

    public static Nibble operator |(Nibble left, Nibble right)
    {
        return new Nibble(
            left.B3 || right.B3,
            left.B2 || right.B2,
            left.B1 || right.B1,
            left.B0 || right.B0);
    }

    public static Nibble operator ^(Nibble left, Nibble right)
    {
        return new Nibble(
            left.B3 ^ right.B3,
            left.B2 ^ right.B2,
            left.B1 ^ right.B1,
            left.B0 ^ right.B0);
    }

    public bool Equals(Nibble other)
    {
        return B3 == other.B3 &&
               B2 == other.B2 &&
               B1 == other.B1 &&
               B0 == other.B0;
    }

    public override bool Equals(object? obj)
    {
        return obj is Nibble other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(B3, B2, B1, B0);
    }

    public static bool operator ==(Nibble left, Nibble right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Nibble left, Nibble right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return ((BigInteger) this).ToString();
    }
}
