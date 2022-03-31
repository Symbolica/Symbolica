using System;
using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Exceptions;

[Serializable]
public class UnsupportedFunctionException : UnsupportedException
{
    public UnsupportedFunctionException(string name, IStack stack)
        : base($"Function '{name}' is unsupported.")
    {
        Name = name;
        UserStackTrace = stack.Trace;
    }

    public string Name { get; }

    public IEnumerable<string> UserStackTrace { get; }
}
