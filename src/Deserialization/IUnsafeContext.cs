using LLVMSharp.Interop;

namespace Symbolica.Deserialization;

internal interface IUnsafeContext
{
    LLVMMemoryBufferRef GetMemoryBuffer(byte[] bytes);
    LLVMTargetDataRef GetTargetData(LLVMModuleRef module);
    uint GetPointerSize(LLVMTargetDataRef targetData);
    LLVMTypeRef GetAllocatedType(LLVMValueRef instruction);
    LLVMValueRef GetAlias(LLVMValueRef operand);
    uint GetEnumAttributeKind(LLVMAttributeRef attribute);
    uint GetEnumAttributeKind(string name);
}
