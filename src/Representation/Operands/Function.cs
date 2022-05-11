using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class Function : IOperand
{
    private readonly FunctionId _id;

    public Function(FunctionId id)
    {
        _id = id;
    }

    public IExpression Evaluate(IExpressionFactory exprFactory, IState state)
    {
        return exprFactory.CreateConstant(exprFactory.PointerSize, (ulong) _id);
    }
}
