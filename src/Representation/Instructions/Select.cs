using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class Select : IInstruction
{
    private readonly IOperand[] _operands;

    public Select(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IExpressionFactory exprFactory, IState state)
    {
        var predicate = _operands[0].Evaluate(exprFactory, state);
        var trueValue = _operands[1].Evaluate(exprFactory, state);
        var falseValue = _operands[2].Evaluate(exprFactory, state);
        var result = predicate.Select(trueValue, falseValue);

        state.Stack.SetVariable(Id, result);
    }
}
