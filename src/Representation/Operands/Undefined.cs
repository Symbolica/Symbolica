using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class Undefined : IOperand
{
    private readonly Size _size;

    public Undefined(Size size)
    {
        _size = size;
    }

    public IExpression Evaluate(IState state)
    {
        return state.Space.CreateGarbage(_size);
    }
}
