using System.Linq;
using LLVMSharp.Interop;
using Symbolica.Abstraction;
using Symbolica.Deserialization.Extensions;
using Symbolica.Expression;
using Symbolica.Representation;

namespace Symbolica.Deserialization
{
    internal sealed class ModuleFactory
    {
        private readonly IFunctionFactory _functionFactory;
        private readonly IGlobalFactory _globalFactory;
        private readonly IStructTypeFactory _structTypeFactory;

        public ModuleFactory(IStructTypeFactory structTypeFactory,
            IFunctionFactory functionFactory, IGlobalFactory globalFactory)
        {
            _structTypeFactory = structTypeFactory;
            _functionFactory = functionFactory;
            _globalFactory = globalFactory;
        }

        public IModule Create(LLVMModuleRef module, Bytes pointerSize)
        {
            return new Module(
                module.Target,
                pointerSize.ToBits(),
                _structTypeFactory.Create(module, "struct.__dirstream"),
                _structTypeFactory.Create(module, "struct.dirent"),
                _structTypeFactory.Create(module, "struct.__jmp_buf_tag"),
                _structTypeFactory.Create(module, "struct.__locale_struct"),
                _structTypeFactory.Create(module, "struct.stat"),
                _structTypeFactory.Create(module, "struct.__pthread"),
                _structTypeFactory.Create(module, "struct.__va_list_tag"),
                module.GetFunctions()
                    .Select(_functionFactory.Create)
                    .ToArray(),
                module.GetGlobals()
                    .Select(_globalFactory.Create)
                    .ToArray());
        }
    }
}
