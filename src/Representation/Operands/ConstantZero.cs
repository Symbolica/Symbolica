using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Operands;

public sealed class ConstantZero : IOperand
{
    private readonly Bits _size;

    public ConstantZero(Bits size)
    {
        _size = size;
    }

    public IExpression<IType> Evaluate(IState state)
    {
        return ConstantUnsigned.CreateZero(_size);
    }
}
