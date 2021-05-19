using Symbolica.Abstraction;
using Symbolica.Representation.Operands;

namespace Symbolica.Deserialization.DTOs.Operands
{
    internal sealed record VariableDto(
            string Type, ulong InstructionId)
        : OperandDto(Type)
    {
        public IOperand To()
        {
            return new Variable((InstructionId) InstructionId);
        }
    }
}