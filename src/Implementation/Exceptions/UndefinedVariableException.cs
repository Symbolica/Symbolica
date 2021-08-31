using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Exceptions
{
    [Serializable]
    public class UndefinedVariableException : ErrorException
    {
        public UndefinedVariableException(InstructionId id)
            : base($"Variable {id} is undefined.")
        {
            Id = id;
        }

        public InstructionId Id { get; }
    }
}
