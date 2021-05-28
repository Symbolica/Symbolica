using System;

namespace Symbolica.Expression
{
    public readonly struct Bytes : IEquatable<Bytes>, IComparable<Bytes>
    {
        private readonly uint _value;

        private Bytes(uint value)
        {
            _value = value;
        }

        public static Bytes Zero => (Bytes) 0U;
        public static Bytes One => (Bytes) 1U;

        public static explicit operator Bytes(uint bytes)
        {
            return new(bytes);
        }

        public static explicit operator uint(Bytes bytes)
        {
            return bytes._value;
        }

        public Bits ToBits()
        {
            return (Bits) (8U * _value);
        }

        public Bytes AlignTo(Bytes bytes)
        {
            return (Bytes) (bytes._value * ((_value + bytes._value - 1U) / bytes._value));
        }

        public static Bytes operator +(Bytes left, Bytes right)
        {
            return (Bytes) (left._value + right._value);
        }

        public bool Equals(Bytes other)
        {
            return _value == other._value;
        }

        public override bool Equals(object? obj)
        {
            return obj is Bytes other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(Bytes left, Bytes right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Bytes left, Bytes right)
        {
            return !left.Equals(right);
        }

        public int CompareTo(Bytes other)
        {
            return _value.CompareTo(other._value);
        }

        public static bool operator <(Bytes left, Bytes right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Bytes left, Bytes right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(Bytes left, Bytes right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(Bytes left, Bytes right)
        {
            return left.CompareTo(right) >= 0;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
