using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions;

public sealed class FloatOrderedAndLess : IInstruction
{
    private readonly IOperand[] _operands;

    public FloatOrderedAndLess(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var left = _operands[0].Evaluate(state);
        var right = _operands[1].Evaluate(state);
        var result = Expression.Values.And.Create(
            Expression.Values.FloatOrdered.Create(left, right),
            Expression.Values.FloatLess.Create(left, right));

        state.Stack.SetVariable(Id, result);
    }
}
