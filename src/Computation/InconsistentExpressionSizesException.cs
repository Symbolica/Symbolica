using System;
using Symbolica.Expression;

namespace Symbolica.Computation
{
    [Serializable]
    public class InconsistentExpressionSizesException : Exception
    {
        public InconsistentExpressionSizesException(Bits left, Bits right)
        {
            Left = left;
            Right = right;
        }

        public Bits Left { get; }
        public Bits Right { get; }
    }
}
