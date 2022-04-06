using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IOperand
{
    IExpression<IType> Evaluate(IState state);
}
