using System;

namespace Symbolica.Expression
{
    public readonly struct Bits : IEquatable<Bits>, IComparable<Bits>
    {
        private readonly uint _value;

        private Bits(uint value)
        {
            _value = value;
        }

        public static Bits Zero => (Bits) 0U;
        public static Bits One => (Bits) 1U;

        public static explicit operator Bits(uint bits)
        {
            return new(bits);
        }

        public static explicit operator uint(Bits bits)
        {
            return bits._value;
        }

        public Bytes ToBytes()
        {
            return (Bytes) ((7U + _value) / 8U);
        }

        public static Bits operator +(Bits left, Bits right)
        {
            return (Bits) (left._value + right._value);
        }

        public static Bits operator -(Bits left, Bits right)
        {
            return (Bits) (left._value - right._value);
        }

        public bool Equals(Bits other)
        {
            return _value == other._value;
        }

        public override bool Equals(object? obj)
        {
            return obj is Bits other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(Bits left, Bits right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Bits left, Bits right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(Bits other)
        {
            return _value.CompareTo(other._value);
        }

        public static bool operator <(Bits left, Bits right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Bits left, Bits right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(Bits left, Bits right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(Bits left, Bits right)
        {
            return left.CompareTo(right) >= 0;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
