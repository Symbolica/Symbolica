using Symbolica.Abstraction;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Instructions;

public sealed class ArithmeticShiftRight : IInstruction
{
    private readonly IOperand[] _operands;

    public ArithmeticShiftRight(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var left = _operands[0].Evaluate(state);
        var right = _operands[1].Evaluate(state);

        var isUndefined = Expression.Values.UnsignedGreaterOrEqual.Create(
            right,
            ConstantUnsigned.Create(right.Size, (uint) left.Size));
        using var proposition = state.Space.CreateProposition(isUndefined);

        if (proposition.CanBeTrue())
            throw new StateException(StateError.UndefinedShift, proposition.CreateTrueSpace());

        var result = Expression.Values.ArithmeticShiftRight.Create(left, right);

        state.Stack.SetVariable(Id, result);
    }
}
