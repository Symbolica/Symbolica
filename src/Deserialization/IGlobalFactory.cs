using LLVMSharp.Interop;
using Symbolica.Abstraction;
namespace Symbolica.Deserialization;

internal interface IGlobalFactory
{
    IGlobal Create(LLVMValueRef global);
}
