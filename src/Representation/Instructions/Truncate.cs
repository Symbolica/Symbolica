﻿using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class Truncate : IInstruction
{
    private readonly IOperand[] _operands;
    private readonly Bits _size;

    public Truncate(InstructionId id, IOperand[] operands, Bits size)
    {
        Id = id;
        _operands = operands;
        _size = size;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var expression = _operands[0].Evaluate(state);
        var result = Expression.Values.Truncate.Create(_size, expression);

        state.Stack.SetVariable(Id, result);
    }
}
