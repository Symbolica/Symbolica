using Symbolica.Expression;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentAllocations : IAllocations, IEquivalent<ExpressionSubs, IPersistentAllocations>, IMergeable<IPersistentAllocations>
{
    IPersistentAllocations Add(IExpression allocation);
}
