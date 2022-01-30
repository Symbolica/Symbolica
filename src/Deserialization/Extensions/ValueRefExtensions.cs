using System.Collections.Generic;
using System.Linq;
using LLVMSharp.Interop;

namespace Symbolica.Deserialization.Extensions;

internal static class ValueRefExtensions
{
    public static string GetValue(this LLVMValueRef self)
    {
        return self.ToString().Split(' ').Last();
    }

    public static IEnumerable<LLVMValueRef> GetOperands(this LLVMValueRef self)
    {
        return Enumerable.Range(0, self.OperandCount)
            .Select(i => self.GetOperand((uint) i));
    }

    public static IEnumerable<LLVMAttributeIndex> GetAttributeIndices(this LLVMValueRef self)
    {
        return Enumerable.Range(0, self.OperandCount)
            .Skip(1)
            .Select(i => (LLVMAttributeIndex) i);
    }

    public static IEnumerable<LLVMBasicBlockRef> GetIncomingBasicBlocks(this LLVMValueRef self)
    {
        return Enumerable.Range(0, (int) self.IncomingCount)
            .Select(i => self.GetIncomingBlock((uint) i));
    }

    public static IEnumerable<LLVMValueRef> GetConstants(this LLVMValueRef self)
    {
        return Enumerable.Range(0, (int) self.TypeOf.ArrayLength)
            .Select(i => self.GetElementAsConstant((uint) i));
    }
}
