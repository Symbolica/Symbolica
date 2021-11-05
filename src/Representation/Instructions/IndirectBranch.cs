using System.Numerics;
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

            state.ForkAll(successorId, new TransferByIdAction());
        }

        private class TransferByIdAction : IForkAllAction
        {
            public IStateAction Run(BigInteger value) =>
                new TransferBasicBlock((BasicBlockId)(ulong)value);
        }

        private class TransferBasicBlock : IStateAction
        {
            private readonly BasicBlockId _successorId;

            public TransferBasicBlock(BasicBlockId successorId)
            {
                _successorId = successorId;
            }

            public Unit Run(IState state)
            {
                state.Stack.TransferBasicBlock(_successorId);
                return new Unit();
            }
        }
    }
}
