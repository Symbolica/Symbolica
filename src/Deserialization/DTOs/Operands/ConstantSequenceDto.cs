using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Operands;

namespace Symbolica.Deserialization.DTOs.Operands
{
    internal sealed record ConstantSequenceDto(
            string Type, uint Size, IEnumerable<OperandDto> Elements)
        : OperandDto(Type)
    {
        public IOperand To()
        {
            return new ConstantSequence((Bits) Size, Elements.Convert());
        }
    }
}
