using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IStackProxy : IStack, IMergeable<IExpression, IStackProxy>
{
    IStackProxy Clone();
    void ExecuteNextInstruction(IState state);
}
