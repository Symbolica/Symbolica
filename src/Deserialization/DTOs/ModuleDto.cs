using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Deserialization.DTOs.Functions;
using Symbolica.Expression;
using Symbolica.Representation;

namespace Symbolica.Deserialization.DTOs
{
    internal sealed record ModuleDto(
            string Target,
            uint PointerSize,
            StructTypeDto? DirectoryStreamType,
            StructTypeDto? DirectoryEntryType,
            StructTypeDto? JumpBufferType,
            StructTypeDto? LocaleType,
            StructTypeDto? StatType,
            StructTypeDto? ThreadType,
            StructTypeDto? VaListType,
            IEnumerable<FunctionDto> Functions,
            IEnumerable<GlobalDto> Globals)
        : Serializable<IModule>
    {
        public override IModule Convert()
        {
            return new Module(
                Target,
                (Bits) PointerSize,
                DirectoryStreamType?.Convert(),
                DirectoryEntryType?.Convert(),
                JumpBufferType?.Convert(),
                LocaleType?.Convert(),
                StatType?.Convert(),
                ThreadType?.Convert(),
                VaListType?.Convert(),
                Functions.Convert(),
                Globals.Convert());
        }
    }
}
