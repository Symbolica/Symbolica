using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Operands;

namespace Symbolica.Deserialization.DTOs.Instructions
{
    internal record InstructionDto(
            string Type, ulong Id, IEnumerable<OperandDto> Operands)
        : PolymorphicSerializable<IInstruction>(Type)
    {
        public override IInstruction Convert()
        {
            return InstructionMapper.Map(this);
        }
    }
}
