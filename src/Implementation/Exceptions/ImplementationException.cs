using System;
using Symbolica.Expression;

namespace Symbolica.Implementation.Exceptions
{
    [Serializable]
    public class ImplementationException : SymbolicaRuntimeException
    {
        public ImplementationException(string message)
            : base(message)
        {
        }
    }
}
