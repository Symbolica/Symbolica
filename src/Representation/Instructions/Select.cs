using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
    [Serializable]
    public sealed class Select : IInstruction
    {
        private readonly IOperand[] _operands;

        public Select(InstructionId id, IOperand[] operands)
        {
            Id = id;
            _operands = operands;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var predicate = _operands[0].Evaluate(state);
            var trueValue = _operands[1].Evaluate(state);
            var falseValue = _operands[2].Evaluate(state);
            var result = predicate.Select(trueValue, falseValue);

            state.Stack.SetVariable(Id, result);
        }
    }
}
