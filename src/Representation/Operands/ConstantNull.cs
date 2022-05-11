using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class ConstantNull : IOperand
{
    public IExpression Evaluate(IExpressionFactory exprFactory, IState state)
    {
        return exprFactory.CreateZero(exprFactory.PointerSize);
    }
}
