using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Operands;
using Symbolica.Expression;
using Symbolica.Representation.Instructions;

namespace Symbolica.Deserialization.DTOs.Instructions
{
    internal sealed record LoadDto(
            string Type, ulong Id, IEnumerable<OperandDto> Operands, uint Size)
        : InstructionDto(Type, Id, Operands)
    {
        public IInstruction To(InstructionId id, IOperand[] operands)
        {
            return new Load(id, operands, (Bits) Size);
        }
    }
}
