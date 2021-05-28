using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Operands;
using Symbolica.Expression;
using Symbolica.Representation.Instructions;

namespace Symbolica.Deserialization.DTOs.Instructions
{
    internal sealed record VectorDto(
            string Type, ulong Id, IEnumerable<OperandDto> Operands, uint ElementSize, ulong ElementCount)
        : InstructionDto(Type, Id, Operands)
    {
        public IInstruction ToExtractElement(InstructionId id, IOperand[] operands)
        {
            return new ExtractElement(id, operands, (Bits) ElementSize, ElementCount);
        }

        public IInstruction ToInsertElement(InstructionId id, IOperand[] operands)
        {
            return new InsertElement(id, operands, (Bits) ElementSize, ElementCount);
        }
    }
}
