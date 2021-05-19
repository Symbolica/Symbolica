using Symbolica.Expression;
using Symbolica.Representation;

namespace Symbolica.Deserialization.DTOs
{
    internal sealed record OffsetDto(
            int OperandNumber, uint ElementSize)
        : Serializable<Offset>
    {
        public override Offset Convert()
        {
            return new(OperandNumber, (Bytes) ElementSize);
        }
    }
}