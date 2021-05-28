using Symbolica.Expression;

namespace Symbolica.Implementation.Stack
{
    internal interface IPersistentAllocations : IAllocations
    {
        IPersistentAllocations Add(IExpression allocation);
    }
}
