using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack
{
    internal interface IPersistentVariables
    {
        IExpression Get(InstructionId instructionId, bool useIncomingValue);
        IPersistentVariables Set(InstructionId instructionId, IExpression variable);
        IPersistentVariables TransferBasicBlock();
    }
}