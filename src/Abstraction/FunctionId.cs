using System;

namespace Symbolica.Abstraction
{
    [Serializable]
    public readonly struct FunctionId : IEquatable<FunctionId>
    {
        private readonly ulong _value;

        private FunctionId(ulong value)
        {
            _value = value;
        }

        public static explicit operator FunctionId(ulong id)
        {
            return new(id);
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
    }
}
