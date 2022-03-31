﻿using System;

namespace Symbolica.Expression;

[Serializable]
public class InconsistentExpressionSizesException : SymbolicaException
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
