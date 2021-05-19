using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Instructions;
using Symbolica.Representation.Operands;

namespace Symbolica.Deserialization.DTOs.Operands
{
    internal sealed record ConstantExpressionDto(
            string Type, InstructionDto Instruction)
        : OperandDto(Type)
    {
        public IOperand To()
        {
            return new ConstantExpression(Instruction.Convert());
        }
    }
}