using System;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    [Serializable]
    public class UnsupportedFloatingPointTypeException : Exception
    {
        public UnsupportedFloatingPointTypeException(Bits size)
        {
            Size = size;
        }

        public Bits Size { get; }
    }
}
