﻿using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class FloatOrderedAndEqual : IInstruction
{
    private readonly IOperand[] _operands;

    public FloatOrderedAndEqual(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IExpressionFactory exprFactory, IState state)
    {
        var left = _operands[0].Evaluate(exprFactory, state);
        var right = _operands[1].Evaluate(exprFactory, state);
        var result = left.FloatOrdered(right).And(left.FloatEqual(right));

        state.Stack.SetVariable(Id, result);
    }
}
