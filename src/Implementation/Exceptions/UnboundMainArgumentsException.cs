using System;
using Symbolica.Expression;

namespace Symbolica.Implementation.Exceptions
{
    [Serializable]
    public class UnboundMainArgumentsException : SymbolicaException
    {
        public UnboundMainArgumentsException()
            : base("The 'main' function cannot access any arguments.")
        {
        }
    }
}
