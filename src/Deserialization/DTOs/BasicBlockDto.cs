using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Instructions;
using Symbolica.Representation;

namespace Symbolica.Deserialization.DTOs
{
    internal sealed record BasicBlockDto(
            ulong Id, IEnumerable<InstructionDto> Instructions)
        : Serializable<IBasicBlock>
    {
        public override IBasicBlock Convert()
        {
            return new BasicBlock((BasicBlockId) Id, Instructions.Convert());
        }
    }
}