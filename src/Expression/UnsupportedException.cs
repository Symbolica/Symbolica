using System;

namespace Symbolica.Expression;

[Serializable]
public abstract class UnsupportedException : SymbolicaException
{
    protected UnsupportedException(string message)
        : base(message)
    {
    }
}
