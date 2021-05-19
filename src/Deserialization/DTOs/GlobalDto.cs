using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Operands;
using Symbolica.Expression;
using Symbolica.Representation;

namespace Symbolica.Deserialization.DTOs
{
    internal sealed record GlobalDto(
            ulong Id, uint Size, OperandDto? Initializer)
        : Serializable<IGlobal>
    {
        public override IGlobal Convert()
        {
            return new Global((GlobalId) Id, (Bits) Size, Initializer?.Convert());
        }
    }
}