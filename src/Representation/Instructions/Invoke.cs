using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions
{
    [Serializable]
    public sealed class Invoke : IInstruction, ICaller
    {
        private readonly Call _call;
        private readonly BasicBlockId _successorId;

        public Invoke(Call call, BasicBlockId successorId)
        {
            _call = call;
            _successorId = successorId;
        }

        public Bits Size => _call.Size;
        public IAttributes ReturnAttributes => _call.ReturnAttributes;

        public void Return(IState state)
        {
            state.Stack.TransferBasicBlock(_successorId);
        }

        public InstructionId Id => _call.Id;

        public void Execute(IState state)
        {
            _call.Execute(state);
        }
    }
}
