using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions;

public sealed class Xor : IInstruction
{
    private readonly IOperand[] _operands;

    public Xor(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var left = _operands[0].Evaluate(state);
        var right = _operands[1].Evaluate(state);
        var result = Expression.Values.Xor.Create(left, right);

        state.Stack.SetVariable(Id, result);
    }
}
