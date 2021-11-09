using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
    public sealed class Branch : IInstruction
    {
        private readonly IOperand[] _operands;

        public Branch(InstructionId id, IOperand[] operands)
        {
            Id = id;
            _operands = operands;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            if (_operands.Length == 1)
                BranchUnconditional(state);
            else
                BranchConditional(state);
        }

        private void BranchUnconditional(IState state)
        {
            var successorId = (BasicBlockId)(ulong)_operands[0].Evaluate(state).Constant;

            state.Stack.TransferBasicBlock(successorId);
        }

        private void BranchConditional(IState state)
        {
            var condition = _operands[0].Evaluate(state);
            var falseSuccessorId = (BasicBlockId)(ulong)_operands[1].Evaluate(state).Constant;
            var trueSuccessorId = (BasicBlockId)(ulong)_operands[2].Evaluate(state).Constant;

            state.Fork(condition,
                new StateActions.TransferBasicBlock(trueSuccessorId),
                new StateActions.TransferBasicBlock(falseSuccessorId));
        }
    }
}
