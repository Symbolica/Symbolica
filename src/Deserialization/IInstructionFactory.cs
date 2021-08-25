using LLVMSharp.Interop;
using Symbolica.Abstraction;

namespace Symbolica.Deserialization
{
    internal interface IInstructionFactory
    {
        IInstruction Create(LLVMValueRef instruction, LLVMOpcode opcode);
    }
}
