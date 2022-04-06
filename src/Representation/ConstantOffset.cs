using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation;

public sealed class ConstantOffset : IOperand
{
    private readonly Bytes _size;

    public ConstantOffset(Bytes size)
    {
        _size = size;
    }

    public IExpression<IType> Evaluate(IState state)
    {
        return ConstantUnsigned.Create(state.Space.PointerSize, (uint) _size);
    }
}
