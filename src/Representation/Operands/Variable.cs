using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class Variable : IOperand
{
    private readonly InstructionId _id;

    public Variable(InstructionId id)
    {
        _id = id;
    }

    public IExpression Evaluate(IExpressionFactory exprFactory, IState state)
    {
        return Evaluate(state, false);
    }

    internal IExpression Evaluate(IState state, bool useIncomingValue)
    {
        return state.Stack.GetVariable(_id, useIncomingValue);
    }
}
