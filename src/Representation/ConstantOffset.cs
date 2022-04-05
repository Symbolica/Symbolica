using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public sealed class ConstantOffset : IOperand
{
    private readonly Size _size;

    public ConstantOffset(Size size)
    {
        _size = size;
    }

    public IExpression Evaluate(IState state)
    {
        return state.Space.CreateConstant(state.Space.PointerSize, _size.Bytes);
    }
}
