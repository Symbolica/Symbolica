using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
    [Serializable]
    public sealed class BitCast : IInstruction
    {
        private readonly IOperand[] _operands;

        public BitCast(InstructionId id, IOperand[] operands)
        {
            Id = id;
            _operands = operands;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var result = _operands[0].Evaluate(state);

            state.Stack.SetVariable(Id, result);
        }
    }
}
