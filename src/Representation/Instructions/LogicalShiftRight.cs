using Symbolica.Abstraction;
using Symbolica.Expression;

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

    public void Execute(IExpressionFactory exprFactory, IState state)
    {
        var left = _operands[0].Evaluate(exprFactory, state);
        var right = _operands[1].Evaluate(exprFactory, state);

        var isUndefined = right.UnsignedGreaterOrEqual(exprFactory.CreateConstant(right.Size, (uint) left.Size));
        using var proposition = isUndefined.GetProposition(state.Space);

        if (proposition.CanBeTrue())
            throw new StateException(StateError.UndefinedShift, proposition.CreateTrueSpace());

        var result = left.LogicalShiftRight(right);

        state.Stack.SetVariable(Id, result);
    }
}
