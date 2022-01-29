using System;
using System.Linq;
using System.Text;
using LLVMSharp.Interop;

namespace Symbolica.Deserialization;

internal sealed unsafe class UnsafeContext : IUnsafeContext
{
    public LLVMMemoryBufferRef GetMemoryBuffer(byte[] bytes)
    {
        fixed (byte* p = bytes)
        {
            return LLVM.CreateMemoryBufferWithMemoryRangeCopy((sbyte*) p, new UIntPtr((ulong) bytes.Length), null);
        }
    }

    public LLVMTargetDataRef GetTargetData(LLVMModuleRef module)
    {
        return LLVM.GetModuleDataLayout(module);
    }

    public uint GetPointerSize(LLVMTargetDataRef targetData)
    {
        return LLVM.PointerSize(targetData);
    }

    public LLVMTypeRef GetAllocatedType(LLVMValueRef instruction)
    {
        return LLVM.GetAllocatedType(instruction);
    }

    public uint[] GetIndices(LLVMValueRef instruction)
    {
        var count = LLVM.GetNumIndices(instruction);
        var p = LLVM.GetIndices(instruction);

        var indices = new uint[count];

        foreach (var index in Enumerable.Range(0, (int) count))
            indices[index] = *p++;

        return indices;
    }

    public LLVMValueRef GetAlias(LLVMValueRef operand)
    {
        return LLVM.AliasGetAliasee(operand);
    }

    public uint GetEnumAttributeKind(LLVMAttributeRef attribute)
    {
        return LLVM.GetEnumAttributeKind(attribute);
    }

    public uint GetEnumAttributeKind(string name)
    {
        fixed (byte* p = Encoding.UTF8.GetBytes(name))
        {
            return LLVM.GetEnumAttributeKindForName((sbyte*) p, new UIntPtr((ulong) name.Length));
        }
    }
}
