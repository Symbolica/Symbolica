using System;

namespace Symbolica.Expression
{
    [Serializable]
    public class SymbolicaException : Exception
    {
        protected SymbolicaException(string message)
            : base(message)
        {
        }
    }
}
