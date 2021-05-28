using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.Stack
{
    internal interface IStackProxy : IStack
    {
        IStackProxy Clone(ISpace space, IMemoryProxy memory);
        void ExecuteNextInstruction(IState state);
    }
}
