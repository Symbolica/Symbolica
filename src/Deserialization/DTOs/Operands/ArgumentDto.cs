using Symbolica.Abstraction;
using Symbolica.Representation.Operands;

namespace Symbolica.Deserialization.DTOs.Operands
{
    internal sealed record ArgumentDto(
            string Type, uint Index)
        : OperandDto(Type)
    {
        public IOperand To()
        {
            return new Argument((int) Index);
        }
    }
}
