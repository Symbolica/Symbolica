using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions;

public sealed class LogicalShiftRight : IInstruction
{
    private readonly IOperand[] _operands;

    public LogicalShiftRight(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var left = _operands[0].Evaluate(state);
        var right = _operands[1].Evaluate(state);

        var isUndefined = right.UnsignedGreaterOrEqual(state.Space.CreateConstant(right.Size, left.Size.Bits));
        using var proposition = isUndefined.GetProposition(state.Space);

        if (proposition.CanBeTrue())
            throw new StateException(StateError.UndefinedShift, proposition.CreateTrueSpace());

        var result = left.LogicalShiftRight(right);

        state.Stack.SetVariable(Id, result);
    }
}
