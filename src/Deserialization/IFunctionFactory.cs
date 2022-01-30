using LLVMSharp.Interop;
using Symbolica.Abstraction;

namespace Symbolica.Deserialization;

internal interface IFunctionFactory
{
    IFunction Create(LLVMValueRef function);
}
