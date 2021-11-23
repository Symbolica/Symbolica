using System;
using System.Linq;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
    [Serializable]
    public sealed class GetElementPointer : IInstruction
    {
        private readonly IOperand[] _offsets;
        private readonly IOperand[] _operands;

        public GetElementPointer(InstructionId id, IOperand[] operands, IOperand[] offsets)
        {
            Id = id;
            _operands = operands;
            _offsets = offsets;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var address = _operands[0].Evaluate(state);
            var result = _offsets.Aggregate(address, (l, r) => l.Add(r.Evaluate(state)));

            state.Stack.SetVariable(Id, result);
        }
    }
}
