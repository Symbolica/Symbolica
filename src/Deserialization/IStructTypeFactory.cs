using LLVMSharp.Interop;
using Symbolica.Abstraction;

namespace Symbolica.Deserialization
{
    internal interface IStructTypeFactory
    {
        IStructType? Create(LLVMModuleRef module, string name);
    }
}
