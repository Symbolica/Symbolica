using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class SignedDivide : IInstruction
{
    private readonly IOperand[] _operands;

    public SignedDivide(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IExpressionFactory exprFactory, IState state)
    {
        var left = _operands[0].Evaluate(exprFactory, state);
        var right = _operands[1].Evaluate(exprFactory, state);

        using var proposition = right.GetProposition(state.Space);

        if (proposition.CanBeFalse())
            throw new StateException(StateError.DivideByZero, proposition.CreateFalseSpace());

        var result = left.SignedDivide(right);

        state.Stack.SetVariable(Id, result);
    }
}
