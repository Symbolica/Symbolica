using System;
using Symbolica.Expression;

namespace Symbolica.Computation.Exceptions;

[Serializable]
public class UnsupportedFloatingPointTypeException : UnsupportedException
{
    public UnsupportedFloatingPointTypeException(Size size)
        : base($"Floating-point size {size} is unsupported.")
    {
        Size = size;
    }

    public Size Size { get; }
}
