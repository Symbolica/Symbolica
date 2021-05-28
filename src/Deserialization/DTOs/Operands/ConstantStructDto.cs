using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Operands;

namespace Symbolica.Deserialization.DTOs.Operands
{
    internal sealed record ConstantStructDto(
            string Type, uint Size, IEnumerable<StructElementDto> Elements)
        : OperandDto(Type)
    {
        public IOperand To()
        {
            return new ConstantStruct((Bits) Size, Elements.Convert());
        }
    }
}
