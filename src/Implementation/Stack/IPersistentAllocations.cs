using Symbolica.Expression.Values;

namespace Symbolica.Implementation.Stack;

internal interface IPersistentAllocations : IAllocations
{
    IPersistentAllocations Add(Address allocation);
}
