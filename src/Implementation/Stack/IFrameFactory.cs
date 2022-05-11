using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Implementation.Memory;

namespace Symbolica.Implementation.Stack;

internal interface IFrameFactory
{
    IPersistentFrame Create(ISpace space, IMemory memory, ICaller caller, IInvocation invocation);
    IPersistentFrame CreateInitial(IDefinition main);
}
