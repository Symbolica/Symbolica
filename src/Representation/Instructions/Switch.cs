using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
    [Serializable]
    public sealed class Switch : IInstruction
    {
        private readonly IOperand[] _operands;

        public Switch(InstructionId id, IOperand[] operands)
        {
            Id = id;
            _operands = operands;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            StateActions.Switch.Run(_operands, state);
        }
    }
}
