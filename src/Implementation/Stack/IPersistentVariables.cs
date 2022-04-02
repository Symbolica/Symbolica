using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentVariables
{
    IExpression<IType> Get(InstructionId id, bool useIncomingValue);
    IPersistentVariables Set(InstructionId id, IExpression<IType> variable);
    IPersistentVariables TransferBasicBlock();
}
