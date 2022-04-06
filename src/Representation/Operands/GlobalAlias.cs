using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class GlobalAlias : IOperand
{
    private readonly IOperand _operand;

    public GlobalAlias(IOperand operand)
    {
        _operand = operand;
    }

    public IExpression<IType> Evaluate(IState state)
    {
        return _operand.Evaluate(state);
    }
}
