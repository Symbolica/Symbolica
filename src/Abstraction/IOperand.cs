using Symbolica.Expression;

namespace Symbolica.Abstraction
{
    public interface IOperand
    {
        IExpression Evaluate(IState state);
    }
}
