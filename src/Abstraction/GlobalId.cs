using System;

namespace Symbolica.Abstraction
{
    public readonly struct GlobalId : IEquatable<GlobalId>
    {
        private readonly ulong _value;

        private GlobalId(ulong value)
        {
            _value = value;
        }

        public static explicit operator GlobalId(ulong id)
        {
            return new(id);
        }

        public static explicit operator ulong(GlobalId id)
        {
            return id._value;
        }

        public bool Equals(GlobalId other)
        {
            return _value == other._value;
        }

        public override bool Equals(object? obj)
        {
            return obj is GlobalId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(GlobalId left, GlobalId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GlobalId left, GlobalId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
