using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions;

public sealed class FloatOrderedAndGreater : IInstruction
{
    private readonly IOperand[] _operands;

    public FloatOrderedAndGreater(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var left = _operands[0].Evaluate(state);
        var right = _operands[1].Evaluate(state);
        var result = left.FloatOrdered(right).And(left.FloatGreater(right));

        state.Stack.SetVariable(Id, result);
    }
}
