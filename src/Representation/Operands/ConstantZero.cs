using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class ConstantZero : IOperand
{
    private readonly Bits _size;

    public ConstantZero(Bits size)
    {
        _size = size;
    }

    public IExpression Evaluate(IState state)
    {
        return state.Space.CreateZero(_size);
    }
}
