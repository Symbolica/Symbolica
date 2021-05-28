using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Operands;
using Symbolica.Expression;
using Symbolica.Representation.Instructions;

namespace Symbolica.Deserialization.DTOs.Instructions
{
    internal sealed record GetElementPointerDto(
            string Type, ulong Id, IEnumerable<OperandDto> Operands,
            IEnumerable<uint> ConstantOffsets, IEnumerable<OffsetDto> Offsets)
        : InstructionDto(Type, Id, Operands)
    {
        public IInstruction To(InstructionId id, IOperand[] operands)
        {
            return new GetElementPointer(id, operands,
                ConstantOffsets.Select(o => (Bytes) o).ToArray(), Offsets.Convert());
        }
    }
}
