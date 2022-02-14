using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Exceptions;

[Serializable]
public class InvalidModelException : SymbolicaException
{
    public InvalidModelException(Status status)
        : base($"A valid model cannot be produced for satisfiability {status}.")
    {
        Status = status;
    }

    public Status Status { get; }
}
