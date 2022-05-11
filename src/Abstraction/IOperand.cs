using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IOperand
{
    IExpression Evaluate(IExpressionFactory exprFactory, IState state);
}
