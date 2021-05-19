using Symbolica.Abstraction;

namespace Symbolica.Deserialization.DTOs.Operands
{
    internal record OperandDto(
            string Type)
        : PolymorphicSerializable<IOperand>(Type)
    {
        public override IOperand Convert()
        {
            return OperandMapper.Map(this);
        }
    }
}