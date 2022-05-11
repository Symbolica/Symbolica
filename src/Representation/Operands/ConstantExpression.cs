using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class ConstantExpression : IOperand
{
    private readonly IInstruction _instruction;

    public ConstantExpression(IInstruction instruction)
    {
        _instruction = instruction;
    }

    public IExpression Evaluate(IExpressionFactory exprFactory, IState state)
    {
        _instruction.Execute(exprFactory, state);

        return state.Stack.GetVariable(_instruction.Id, false);
    }
}
