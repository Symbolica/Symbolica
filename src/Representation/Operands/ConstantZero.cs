using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class ConstantZero : IOperand
{
    private readonly Size _size;

    public ConstantZero(Size size)
    {
        _size = size;
    }

    public IExpression Evaluate(IState state)
    {
        return state.Space.CreateZero(_size);
    }
}
