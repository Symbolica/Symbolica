using LLVMSharp.Interop;
using Symbolica.Abstraction;

namespace Symbolica.Deserialization;

internal interface ITypeFactory
{
    TType Create<TType>(LLVMTypeRef type)
        where TType : class, IType;
}
