﻿using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
    [Serializable]
    public sealed class FloatAdd : IInstruction
    {
        private readonly IOperand[] _operands;

        public FloatAdd(InstructionId id, IOperand[] operands)
        {
            Id = id;
            _operands = operands;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var left = _operands[0].Evaluate(state);
            var right = _operands[1].Evaluate(state);
            var result = left.FloatAdd(right);

            state.Stack.SetVariable(Id, result);
        }
    }
}
