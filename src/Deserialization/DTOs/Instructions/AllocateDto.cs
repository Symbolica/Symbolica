using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Operands;
using Symbolica.Expression;
using Symbolica.Representation.Instructions;

namespace Symbolica.Deserialization.DTOs.Instructions
{
    internal sealed record AllocateDto(
            string Type, ulong Id, IEnumerable<OperandDto> Operands, uint ElementSize)
        : InstructionDto(Type, Id, Operands)
    {
        public IInstruction To(InstructionId id, IOperand[] operands)
        {
            return new Allocate(id, operands, (Bits) ElementSize);
        }
    }
}