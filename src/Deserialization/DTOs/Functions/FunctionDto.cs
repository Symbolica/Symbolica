using System.Collections.Generic;
using Symbolica.Abstraction;

namespace Symbolica.Deserialization.DTOs.Functions
{
    internal record FunctionDto(
            string Type, ulong Id, string Name, IEnumerable<ParameterDto> Parameters)
        : PolymorphicSerializable<IFunction>(Type)
    {
        public override IFunction Convert()
        {
            return FunctionMapper.Map(this);
        }
    }
}
