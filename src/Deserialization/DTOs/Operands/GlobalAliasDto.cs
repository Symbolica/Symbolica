using Symbolica.Abstraction;
using Symbolica.Representation.Operands;

namespace Symbolica.Deserialization.DTOs.Operands
{
    internal sealed record GlobalAliasDto(
            string Type, OperandDto Operand)
        : OperandDto(Type)
    {
        public IOperand To()
        {
            return new GlobalAlias(Operand.Convert());
        }
    }
}
