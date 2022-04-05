using LLVMSharp.Interop;
using Symbolica.Expression;

namespace Symbolica.Deserialization.Extensions;

internal static class TypeRefExtensions
{
    public static Size GetSize(this LLVMTypeRef self, LLVMTargetDataRef targetData)
    {
        return Size.FromBits(targetData.SizeOfTypeInBits(self));
    }

    public static Size GetAllocSize(this LLVMTypeRef self, LLVMTargetDataRef targetData)
    {
        return Size.FromBytes(targetData.ABISizeOfType(self));
    }

    public static Size GetStoreSize(this LLVMTypeRef self, LLVMTargetDataRef targetData)
    {
        return self.IsSized
            ? Size.FromBytes(targetData.StoreSizeOfType(self))
            : Size.Zero;
    }

    public static Size GetElementOffset(this LLVMTypeRef self, LLVMTargetDataRef targetData, uint element)
    {
        return Size.FromBytes(targetData.OffsetOfElement(self, element));
    }
}
