using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Operands;

public sealed class ConstantInteger : IOperand
{
    private readonly Bits _size;
    private readonly BigInteger _value;

    public ConstantInteger(Bits size, BigInteger value)
    {
        _size = size;
        _value = value;
    }

    public IExpression<IType> Evaluate(IState state)
    {
        return ConstantUnsigned.Create(_size, _value);
    }
}
