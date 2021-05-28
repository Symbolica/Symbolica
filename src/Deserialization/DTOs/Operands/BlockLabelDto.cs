using Symbolica.Abstraction;
using Symbolica.Representation.Operands;

namespace Symbolica.Deserialization.DTOs.Operands
{
    internal sealed record BlockLabelDto(
            string Type, ulong BasicBlockId)
        : OperandDto(Type)
    {
        public IOperand To()
        {
            return new BlockLabel((BasicBlockId) BasicBlockId);
        }
    }
}
