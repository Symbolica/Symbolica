using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class FloatNegate : IInstruction
{
    private readonly IOperand[] _operands;

    public FloatNegate(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IExpressionFactory exprFactory, IState state)
    {
        var expression = _operands[0].Evaluate(exprFactory, state);
        var result = expression.FloatNegate();

        state.Stack.SetVariable(Id, result);
    }
}
