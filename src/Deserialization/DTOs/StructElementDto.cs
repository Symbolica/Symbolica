using Symbolica.Deserialization.DTOs.Operands;
using Symbolica.Expression;
using Symbolica.Representation;

namespace Symbolica.Deserialization.DTOs
{
    internal sealed record StructElementDto(
            uint Offset, OperandDto Operand)
        : Serializable<StructElement>
    {
        public override StructElement Convert()
        {
            return new((Bits) Offset, Operand.Convert());
        }
    }
}
