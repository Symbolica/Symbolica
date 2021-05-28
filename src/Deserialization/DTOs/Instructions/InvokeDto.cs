using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Operands;
using Symbolica.Representation.Instructions;

namespace Symbolica.Deserialization.DTOs.Instructions
{
    internal sealed record InvokeDto(
            string Type, ulong Id, IEnumerable<OperandDto> Operands,
            CallDto Call, ulong SuccessorId)
        : InstructionDto(Type, Id, Operands)
    {
        public IInstruction To(InstructionId id, IOperand[] operands)
        {
            var call = Call.To(id, operands);

            return new Invoke(call, (BasicBlockId) SuccessorId);
        }
    }
}
