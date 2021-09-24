using System;
using Symbolica.Expression;

namespace Symbolica.Computation.Exceptions
{
    [Serializable]
    public class UnsupportedSymbolicArithmeticException : ErrorException
    {
        public UnsupportedSymbolicArithmeticException()
            : base("The symbolic arithmetic cannot be converted to machine arithmetic.")
        {
        }
    }
}
