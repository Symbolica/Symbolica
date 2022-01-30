using Symbolica.Expression;

namespace Symbolica.Abstraction;

public readonly struct Parameter
{
    public Parameter(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }
}
