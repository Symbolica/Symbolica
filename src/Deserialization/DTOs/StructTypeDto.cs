using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation;

namespace Symbolica.Deserialization.DTOs
{
    internal sealed record StructTypeDto(
            string Name, uint Size, IEnumerable<uint> Offsets)
        : Serializable<IStructType>
    {
        public override IStructType Convert()
        {
            return new StructType(Name, (Bits) Size, Offsets.Select(o => (Bits) o).ToArray());
        }
    }
}