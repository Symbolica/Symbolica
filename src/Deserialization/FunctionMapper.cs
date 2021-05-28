using System;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Functions;
using Symbolica.Representation;

namespace Symbolica.Deserialization
{
    internal static class FunctionMapper
    {
        public static IFunction Map(FunctionDto dto)
        {
            var id = (FunctionId) dto.Id;
            var parameters = new Parameters(dto.Parameters.Convert());

            return dto.Type switch
            {
                "Declaration" => DeclarationMapper.Map(id, dto.Name, parameters),
                "Definition" => dto.As<DefinitionDto>().To(id, parameters),
                _ => throw new Exception($"Function type {dto.Type} is unknown.")
            };
        }
    }
}
