using LLVMSharp.Interop;
using Symbolica.Abstraction;

namespace Symbolica.Deserialization
{
    internal interface IStructTypeFactory
    {
        IStructType Create(LLVMTypeRef type);
    }
}
