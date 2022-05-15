using System.Collections.Generic;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public readonly struct Parameter : IMergeable<IExpression, Parameter>
{
    public Parameter(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }

    public (HashSet<(IExpression, IExpression)> subs, bool) IsEquivalentTo(Parameter other)
    {
        return (new(), Size == other.Size);
    }
}
