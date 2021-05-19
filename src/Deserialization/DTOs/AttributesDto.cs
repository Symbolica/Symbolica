using Symbolica.Abstraction;
using Symbolica.Representation;

namespace Symbolica.Deserialization.DTOs
{
    internal sealed record AttributesDto(
            bool IsSignExtended)
        : Serializable<IAttributes>
    {
        public override IAttributes Convert()
        {
            return new Attributes(IsSignExtended);
        }
    }
}