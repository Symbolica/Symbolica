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

    public IExpression<IType> Evaluate(IState state)
    {
        return Evaluate(state, false);
    }

    internal IExpression<IType> Evaluate(IState state, bool useIncomingValue)
    {
        return state.Stack.GetVariable(_id, useIncomingValue);
    }
}
