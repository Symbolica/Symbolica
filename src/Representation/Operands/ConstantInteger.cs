using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

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

    public IExpression Evaluate(IExpressionFactory exprFactory, IState state)
    {
        return exprFactory.CreateConstant(_size, _value);
    }
}
