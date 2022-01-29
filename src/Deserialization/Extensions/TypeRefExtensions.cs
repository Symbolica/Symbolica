using LLVMSharp.Interop;
using Symbolica.Expression;

namespace Symbolica.Deserialization.Extensions;

internal static class TypeRefExtensions
{
    public static Bits GetSize(this LLVMTypeRef self, LLVMTargetDataRef targetData)
    {
        return (Bits) targetData.SizeOfTypeInBits(self);
    }

    public static Bytes GetAllocSize(this LLVMTypeRef self, LLVMTargetDataRef targetData)
    {
        return (Bytes) targetData.ABISizeOfType(self);
    }

    public static Bytes GetStoreSize(this LLVMTypeRef self, LLVMTargetDataRef targetData)
    {
        return self.IsSized
            ? (Bytes) targetData.StoreSizeOfType(self)
            : Bytes.Zero;
    }

    public static Bytes GetElementOffset(this LLVMTypeRef self, LLVMTargetDataRef targetData, uint element)
    {
        return (Bytes) targetData.OffsetOfElement(self, element);
    }
}
