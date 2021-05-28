using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Deserialization.DTOs
{
    internal sealed record ParameterDto(
            uint Size)
        : Serializable<Parameter>
    {
        public override Parameter Convert()
        {
            return new((Bits) Size);
        }
    }
}
