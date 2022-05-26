using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public readonly struct Parameter : IMergeable<ExpressionSubs, Parameter>
{
    public Parameter(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }

    public int GetEquivalencyHash(bool includeSubs)
    {
        return Size.GetHashCode();
    }

    public (HashSet<ExpressionSubs> subs, bool) IsEquivalentTo(Parameter other)
    {
        return (new(), Size == other.Size);
    }

    public object ToJson()
    {
        return new { Size = (uint) Size };
    }
}
