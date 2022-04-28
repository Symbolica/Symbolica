using System;
using Symbolica.Expression;

namespace Symbolica.Computation.Exceptions;

[Serializable]
public class UnsupportedExpressionTypeException : SymbolicaException
{
    public UnsupportedExpressionTypeException(Type type)
        : base($"Expression {type} is unsupported.")
    {
        Type = type;
    }

    public Type Type { get; }
}
