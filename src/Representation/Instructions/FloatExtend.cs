using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions
{
    [Serializable]
    public sealed class FloatExtend : IInstruction
    {
        private readonly IOperand[] _operands;
        private readonly Bits _size;

        public FloatExtend(InstructionId id, IOperand[] operands, Bits size)
        {
            Id = id;
            _operands = operands;
            _size = size;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var expression = _operands[0].Evaluate(state);
            var result = expression.FloatConvert(_size);

            state.Stack.SetVariable(Id, result);
        }
    }
}
