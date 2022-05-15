using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentVariables : IMergeable<IExpression, IPersistentVariables>
{
    IExpression Get(InstructionId id, bool useIncomingValue);
    IPersistentVariables Set(InstructionId id, IExpression variable);
    IPersistentVariables TransferBasicBlock();
}
