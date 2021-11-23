﻿using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
    [Serializable]
    public sealed class SignedGreaterOrEqual : IInstruction
    {
        private readonly IOperand[] _operands;

        public SignedGreaterOrEqual(InstructionId id, IOperand[] operands)
        {
            Id = id;
            _operands = operands;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var left = _operands[0].Evaluate(state);
            var right = _operands[1].Evaluate(state);
            var result = left.SignedGreaterOrEqual(right);

            state.Stack.SetVariable(Id, result);
        }
    }
}
