using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Representation.Functions;

namespace Symbolica.Deserialization.DTOs.Functions
{
    internal sealed record DefinitionDto(
            string Type, ulong Id, string Name, IEnumerable<ParameterDto> Parameters,
            bool IsVariadic, IEnumerable<BasicBlockDto> BasicBlocks)
        : FunctionDto(Type, Id, Name, Parameters)
    {
        public IFunction To(FunctionId id, IParameters parameters)
        {
            return new Definition(id, Name, parameters,
                IsVariadic, BasicBlocks.Convert());
        }
    }
}