using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Abstraction;

public interface IOperand
{
    IExpression<IType> Evaluate(IState state);
}
