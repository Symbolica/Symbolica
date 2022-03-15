using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions;

public sealed class UnsignedRemainder : IInstruction
{
    private readonly IOperand[] _operands;

    public UnsignedRemainder(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var left = _operands[0].Evaluate(state);
        var right = _operands[1].Evaluate(state);

        var proposition = right.GetProposition(state.Space);

        if (proposition.CanBeFalse)
        {
            using var space = proposition.FalseSpace;

            throw new StateException(StateError.DivideByZero, space.GetExample());
        }

        var result = left.UnsignedRemainder(right);

        state.Stack.SetVariable(Id, result);
    }
}
