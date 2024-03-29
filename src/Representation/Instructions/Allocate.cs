﻿using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class Allocate : IInstruction
{
    private readonly Bytes _elementSize;
    private readonly IOperand[] _operands;

    public Allocate(InstructionId id, IOperand[] operands, Bytes elementSize)
    {
        Id = id;
        _operands = operands;
        _elementSize = elementSize;
    }

    public InstructionId Id { get; }

    public void Execute(IState state)
    {
        var size = _elementSize * (uint) _operands[0].Evaluate(state).GetSingleValue(state.Space);
        var address = state.Stack.Allocate(size.ToBits());

        state.Stack.SetVariable(Id, address);
    }
}
