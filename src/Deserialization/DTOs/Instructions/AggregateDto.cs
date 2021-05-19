using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Operands;
using Symbolica.Expression;
using Symbolica.Representation.Instructions;

namespace Symbolica.Deserialization.DTOs.Instructions
{
    internal sealed record AggregateDto(
            string Type, ulong Id, IEnumerable<OperandDto> Operands, uint Size, IEnumerable<uint> ConstantOffsets)
        : InstructionDto(Type, Id, Operands)
    {
        public IInstruction ToExtractValue(InstructionId id, IOperand[] operands)
        {
            return new ExtractValue(id, operands, (Bits) Size, ConstantOffsets.Select(o => (Bits) o).ToArray());
        }

        public IInstruction ToInsertValue(InstructionId id, IOperand[] operands)
        {
            return new InsertValue(id, operands, ConstantOffsets.Select(o => (Bits) o).ToArray());
        }
    }
}