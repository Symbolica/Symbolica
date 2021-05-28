using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Operands;

namespace Symbolica.Deserialization.DTOs.Operands
{
    internal sealed record UndefinedDto(
            string Type, uint Size)
        : OperandDto(Type)
    {
        public IOperand To()
        {
            return new Undefined((Bits) Size);
        }
    }
}
