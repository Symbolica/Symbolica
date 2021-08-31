using System;
using Symbolica.Expression;

namespace Symbolica.Computation.Exceptions
{
    [Serializable]
    public class InconsistentExpressionSizesException : SymbolicaRuntimeException
    {
        public InconsistentExpressionSizesException(Bits left, Bits right)
            : base($"Expression sizes {left} and {right} are inconsistent.")
        {
            Left = left;
            Right = right;
        }

        public Bits Left { get; }
        public Bits Right { get; }
    }
}
