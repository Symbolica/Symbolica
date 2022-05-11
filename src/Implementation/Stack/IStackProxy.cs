using Symbolica.Abstraction;

namespace Symbolica.Implementation.Stack;

internal interface IStackProxy : IStack
{
    IStackProxy Clone();
    void ExecuteNextInstruction(IState state);
}
