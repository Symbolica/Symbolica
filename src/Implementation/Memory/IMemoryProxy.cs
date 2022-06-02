using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Implementation.Memory;

internal interface IMemoryProxy : IMemory, IEquivalent<ExpressionSubs, IMemoryProxy>, IMergeable<IMemoryProxy>
{
    IMemoryProxy Clone();
}
