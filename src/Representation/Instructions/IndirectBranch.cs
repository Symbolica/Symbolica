using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
    [Serializable]
    public sealed class IndirectBranch : IInstruction
    {
        private readonly IOperand[] _operands;

        public IndirectBranch(InstructionId id, IOperand[] operands)
        {
            Id = id;
            _operands = operands;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var successorId = _operands[0].Evaluate(state);

            state.ForkAll(successorId, new StateActions.TransferBasicBlockOfId());
        }
    }
}
