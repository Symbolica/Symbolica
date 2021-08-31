using System;
using Symbolica.Expression;

namespace Symbolica.Implementation.Exceptions
{
    [Serializable]
    public class ImplementationException : SymbolicaException
    {
        public ImplementationException(string message)
            : base(message)
        {
        }
    }
}
