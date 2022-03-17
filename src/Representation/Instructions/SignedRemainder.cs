using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions;

public sealed class SignedRemainder : IInstruction
{
    private readonly IOperand[] _operands;

    public SignedRemainder(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var left = _operands[0].Evaluate(state);
        var right = _operands[1].Evaluate(state);

        using var proposition = right.GetProposition(state.Space);

        if (proposition.CanBeFalse())
            throw new StateException(StateError.DivideByZero, proposition.FalseSpace());

        var result = left.SignedRemainder(right);

        state.Stack.SetVariable(Id, result);
    }
}
