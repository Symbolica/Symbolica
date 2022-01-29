using System.Collections.Generic;
using LLVMSharp.Interop;

namespace Symbolica.Deserialization.Extensions;

internal static class BasicBlockRefExtensions
{
    public static IEnumerable<LLVMValueRef> GetInstructions(this LLVMBasicBlockRef self)
    {
        var instruction = self.FirstInstruction;

        while (instruction != self.LastInstruction)
        {
            yield return instruction;
            instruction = instruction.NextInstruction;
        }

        yield return instruction;
    }
}
