using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
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

            state.ForkAll(successorId, (s, v) => Execute(s, (BasicBlockId) (ulong) v));
        }

        private static void Execute(IState state, BasicBlockId successorId)
        {
            state.Stack.TransferBasicBlock(successorId);
        }
    }
}
