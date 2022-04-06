using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class ConstantFloat : IOperand
{
    private readonly Bits _size;
    private readonly string _value;

    public ConstantFloat(Bits size, string value)
    {
        _size = size;
        _value = value;
    }

    public IExpression<IType> Evaluate(IState state)
    {
        return Float.CreateConstant(_size, _value);
    }
}
