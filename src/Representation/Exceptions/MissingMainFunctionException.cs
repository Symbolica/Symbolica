using System;
using Symbolica.Expression;

namespace Symbolica.Representation.Exceptions
{
    [Serializable]
    public class MissingMainFunctionException : SymbolicaException
    {
        public MissingMainFunctionException()
            : base("No 'main' function is defined.")
        {
        }
    }
}
