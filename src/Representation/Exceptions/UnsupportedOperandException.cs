﻿using System;
using Symbolica.Expression;

namespace Symbolica.Representation.Exceptions;

[Serializable]
public class UnsupportedOperandException : UnsupportedException
{
    public UnsupportedOperandException(string type)
        : base($"Operand '{type}' is unsupported.")
    {
        Type = type;
    }

    public string Type { get; }
}
