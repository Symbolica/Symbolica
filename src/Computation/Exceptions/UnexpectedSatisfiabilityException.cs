using System;
using Microsoft.Z3;
using Symbolica.Expression;

namespace Symbolica.Computation.Exceptions;

[Serializable]
public class UnexpectedSatisfiabilityException : SymbolicaException
{
    public UnexpectedSatisfiabilityException(Status status)
        : base($"Satisfiability {status} is unhandled.")
    {
        Status = status;
    }

    public Status Status { get; }
}
