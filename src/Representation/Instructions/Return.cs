﻿using Symbolica.Abstraction;
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

    public void Execute(IState state)
    {
        if (state.Stack.IsInitialFrame)
        {
            ReturnFromInitialFrame(state);
        }
        else
        {
            var caller = _operands.Length == 0
                ? ReturnFromVoid(state)
                : ReturnFromNonVoid(state);

            caller.Return(state);
        }
    }

    private void ReturnFromInitialFrame(IState state)
    {
        var result = _operands[0].Evaluate(state);
        var proposition = result.GetProposition(state.Space);

        if (proposition.CanBeTrue)
        {
            using var space = proposition.TrueSpace;

            throw new StateException(StateError.NonZeroExitCode, space.GetExample());
        }

        state.Complete();
    }

    private static ICaller ReturnFromVoid(IState state)
    {
        return state.Stack.Unwind();
    }

    private ICaller ReturnFromNonVoid(IState state)
    {
        var result = _operands[0].Evaluate(state);

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
