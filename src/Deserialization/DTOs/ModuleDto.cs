using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Functions;
using Symbolica.Expression;
using Symbolica.Representation;

namespace Symbolica.Deserialization.DTOs
{
    internal sealed record ModuleDto(
            string Target, uint PointerSize,
            IEnumerable<StructTypeDto> StructTypes, IEnumerable<FunctionDto> Functions, IEnumerable<GlobalDto> Globals)
        : Serializable<IModule>
    {
        public override IModule Convert()
        {
            return new Module(Target, (Bits) PointerSize,
                StructTypes.Convert(), Functions.Convert(), Globals.Convert());
        }
    }
}
