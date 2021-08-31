using System;
using Symbolica.Expression;

namespace Symbolica.Computation.Exceptions
{
    [Serializable]
    public class IrreducibleSymbolicExpressionException : SymbolicaException
    {
        public IrreducibleSymbolicExpressionException()
            : base("The symbolic expression cannot be reduced to a constant.")
        {
        }
    }
}
