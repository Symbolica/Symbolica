﻿using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions;

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
        var successorId = (BasicBlockId) (ulong) _operands[0].Evaluate(state).GetSingleValue(state.Space);

        state.Stack.TransferBasicBlock(successorId);
    }

    private void BranchConditional(IState state)
    {
        var condition = _operands[0].Evaluate(state);
        var falseSuccessorId = (BasicBlockId) (ulong) _operands[1].Evaluate(state).GetSingleValue(state.Space);
        var trueSuccessorId = (BasicBlockId) (ulong) _operands[2].Evaluate(state).GetSingleValue(state.Space);

        state.Fork(condition,
            new TransferBasicBlock(trueSuccessorId),
            new TransferBasicBlock(falseSuccessorId));
    }

    private sealed class TransferBasicBlock : IStateAction
    {
        private readonly BasicBlockId _id;

        public TransferBasicBlock(BasicBlockId id)
        {
            _id = id;
        }

        public void Invoke(IState state)
        {
            state.Stack.TransferBasicBlock(_id);
        }
    }
}
