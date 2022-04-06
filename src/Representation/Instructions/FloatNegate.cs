using Symbolica.Abstraction;

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

    public void Execute(IState state)
    {
        var expression = _operands[0].Evaluate(state);
        var result = Expression.Values.FloatNegate.Create(expression);

        state.Stack.SetVariable(Id, result);
    }
}
