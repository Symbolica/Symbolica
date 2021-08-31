using System;
using Symbolica.Expression;

namespace Symbolica.Computation.Exceptions
{
    [Serializable]
    public class UnsupportedFloatingPointTypeException : SymbolicaException
    {
        public UnsupportedFloatingPointTypeException(Bits size)
            : base($"Floating-point size {size} is unsupported.")
        {
            Size = size;
        }

        public Bits Size { get; }
    }
}
