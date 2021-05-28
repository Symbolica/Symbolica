using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Operands;
using Symbolica.Expression;
using Symbolica.Representation.Instructions;

namespace Symbolica.Deserialization.DTOs.Instructions
{
    internal sealed record CallDto(
            string Type, ulong Id, IEnumerable<OperandDto> Operands,
            uint Size, AttributesDto Attributes, IEnumerable<AttributesDto> ParameterAttributes)
        : InstructionDto(Type, Id, Operands)
    {
        public Call To(InstructionId id, IOperand[] operands)
        {
            return new(id, operands,
                (Bits) Size, Attributes.Convert(), ParameterAttributes.Convert());
        }
    }
}
