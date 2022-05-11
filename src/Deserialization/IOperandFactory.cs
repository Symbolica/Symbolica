using LLVMSharp.Interop;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Deserialization;

internal interface IOperandFactory
{
    IOperand Create(LLVMValueRef operand, IInstructionFactory instructionFactory);
}
