using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    public interface IModule
    {
        string Target { get; }
        Bits PointerSize { get; }
        IStructType DirectoryStreamType { get; }
        IStructType DirectoryEntryType { get; }
        IStructType JumpBufferType { get; }
        IStructType LocaleType { get; }
        IStructType StatType { get; }
        IStructType ThreadType { get; }
        IStructType VaListType { get; }

        IDefinition GetMain();
        IFunction GetFunction(FunctionId functionId);
        IGlobal GetGlobal(GlobalId globalId);
    }
}
