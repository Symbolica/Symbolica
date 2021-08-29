using System;
using Symbolica.Abstraction;

namespace Symbolica.Implementation
{
    [Serializable]
    public class UndefinedVariableException : Exception
    {
        public UndefinedVariableException(InstructionId id)
        {
            Id = id;
        }

        public InstructionId Id { get; }
    }
}
