using System;
using Symbolica.Expression;

namespace Symbolica.Implementation.Exceptions;

[Serializable]
public class UnboundMainArgumentsException : ErrorException
{
    public UnboundMainArgumentsException()
        : base("The 'main' function cannot access any arguments.")
    {
    }
}
