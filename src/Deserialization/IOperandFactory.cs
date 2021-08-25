using LLVMSharp.Interop;
using Symbolica.Abstraction;

namespace Symbolica.Deserialization
{
    internal interface IOperandFactory
    {
        IOperand Create(LLVMValueRef operand, IInstructionFactory instructionFactory);
    }
}
