using System;

namespace Symbolica.Expression
{
    [Serializable]
    public class SymbolicaRuntimeException : SymbolicaException
    {
        protected SymbolicaRuntimeException(string message)
            : base(message)
        {
        }
    }
}
