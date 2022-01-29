using System;

namespace Symbolica.Expression;

[Serializable]
public abstract class ErrorException : SymbolicaException
{
    protected ErrorException(string message)
        : base(message)
    {
    }
}
