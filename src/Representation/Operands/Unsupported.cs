using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Exceptions;

namespace Symbolica.Representation.Operands;

public sealed class Unsupported : IOperand
{
    private readonly string _type;

    public Unsupported(string type)
    {
        _type = type;
    }

    public IExpression Evaluate(IState state)
    {
        throw new UnsupportedOperandException(_type);
    }
}
