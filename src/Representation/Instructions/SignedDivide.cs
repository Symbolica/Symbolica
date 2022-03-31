﻿using Symbolica.Abstraction;

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

    public void Execute(IState state)
    {
        var left = _operands[0].Evaluate(state);
        var right = _operands[1].Evaluate(state);

        using var proposition = state.Space.CreateProposition(right);

        if (proposition.CanBeFalse())
            throw new StateException(StateError.DivideByZero, proposition.CreateFalseSpace());

        var result = Expression.Values.SignedDivide.Create(left, right);

        state.Stack.SetVariable(Id, result);
    }
}
