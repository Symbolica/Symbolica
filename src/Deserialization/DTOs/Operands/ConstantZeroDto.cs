using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Operands;

namespace Symbolica.Deserialization.DTOs.Operands
{
    internal sealed record ConstantZeroDto(
            string Type, uint Size)
        : OperandDto(Type)
    {
        public IOperand To()
        {
            return new ConstantZero((Bits) Size);
        }
    }
}