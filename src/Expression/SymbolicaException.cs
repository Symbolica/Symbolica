using System;

namespace Symbolica.Expression;

[Serializable]
public abstract class SymbolicaException : Exception
{
    protected SymbolicaException(string message)
        : base(message)
    {
    }
}
