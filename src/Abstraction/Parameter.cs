using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Symbolica.Expression;

namespace Symbolica.Abstraction;

public readonly struct Parameter : IEquivalent<ExpressionSubs, Parameter>, IMergeable<Parameter>
{
    public Parameter(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }

    public int GetEquivalencyHash()
    {
        return Size.GetHashCode();
    }

    public int GetMergeHash()
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

    public bool TryMerge(Parameter other, IExpression predicate, [MaybeNullWhen(false)] out Parameter merged)
    {
        merged = this;
        return Size == other.Size;
    }
}
