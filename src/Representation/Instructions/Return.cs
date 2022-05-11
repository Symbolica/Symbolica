using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class Return : IInstruction
{
    private readonly IOperand[] _operands;

    public Return(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IExpressionFactory exprFactory, IState state)
    {
        if (state.Stack.IsInitialFrame)
        {
            ReturnFromInitialFrame(exprFactory, state);
        }
        else
        {
            var caller = _operands.Length == 0
                ? ReturnFromVoid(state)
                : ReturnFromNonVoid(exprFactory, state);

            caller.Return(state);
        }
    }

    private void ReturnFromInitialFrame(IExpressionFactory exprFactory, IState state)
    {
        var result = _operands[0].Evaluate(exprFactory, state);
        using var proposition = result.GetProposition(state.Space);

        if (proposition.CanBeTrue())
            throw new StateException(StateError.NonZeroExitCode, proposition.CreateTrueSpace());

        state.Complete();
    }

    private static ICaller ReturnFromVoid(IState state)
    {
        return state.Stack.Unwind();
    }

    private ICaller ReturnFromNonVoid(IExpressionFactory exprFactory, IState state)
    {
        var result = _operands[0].Evaluate(exprFactory, state);

        var caller = state.Stack.Unwind();
        state.Stack.SetVariable(caller.Id, Coerce(result, caller));

        return caller;
    }

    private static IExpression Coerce(IExpression expression, ICaller caller)
    {
        return caller.ReturnAttributes.IsSignExtended
            ? expression.SignExtend(caller.Size)
            : expression.ZeroExtend(caller.Size);
    }
}
