using System.Linq;
using LLVMSharp.Interop;
using Symbolica.Abstraction;
using Symbolica.Deserialization.Extensions;
using Symbolica.Expression;
using Symbolica.Representation;

namespace Symbolica.Deserialization;

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
        return Module.Create(
            module.Target,
            pointerSize.ToBits(),
            CreateStructType(module, "struct.__dirstream"),
            CreateStructType(module, "struct.dirent"),
            CreateStructType(module, "struct.__jmp_buf_tag"),
            CreateStructType(module, "struct.__locale_struct"),
            CreateStructType(module, "struct.stat"),
            CreateStructType(module, "struct.__pthread"),
            CreateStructType(module, "struct.__va_list_tag"),
            module.GetFunctions().Select(_functionFactory.Create),
            module.GetGlobals().Select(_globalFactory.Create));
    }

    private (string, IStructType?) CreateStructType(LLVMModuleRef module, string name)
    {
        var type = module.GetTypeByName(name);

        return (name, type == default
            ? null
            : _structTypeFactory.Create(type));
    }
}
