using Symbolica.Expression;

namespace Symbolica.Abstraction;

public readonly struct Parameter
{
    public Parameter(Size size)
    {
        Size = size;
    }

    public Size Size { get; }
}
