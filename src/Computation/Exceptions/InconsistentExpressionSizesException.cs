using System;
using Symbolica.Expression;

namespace Symbolica.Computation.Exceptions;

[Serializable]
public class InconsistentExpressionSizesException : SymbolicaException
{
    public InconsistentExpressionSizesException(Size left, Size right)
        : base($"Expression sizes {left} and {right} are inconsistent.")
    {
        Left = left;
        Right = right;
    }

    public Size Left { get; }
    public Size Right { get; }
}
