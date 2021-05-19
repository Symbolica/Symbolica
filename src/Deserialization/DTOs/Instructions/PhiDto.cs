using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Operands;
using Symbolica.Representation.Instructions;

namespace Symbolica.Deserialization.DTOs.Instructions
{
    internal sealed record PhiDto(
            string Type, ulong Id, IEnumerable<OperandDto> Operands, IEnumerable<ulong> PredecessorIds)
        : InstructionDto(Type, Id, Operands)
    {
        public IInstruction To(InstructionId id, IOperand[] operands)
        {
            return new Phi(id, operands, PredecessorIds.Select(i => (BasicBlockId) i));
        }
    }
}