using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions
{
    public sealed class PointerToInteger : IInstruction
    {
        private readonly IOperand[] _operands;
        private readonly Bits _size;

        public PointerToInteger(InstructionId id, IOperand[] operands, Bits size)
        {
            Id = id;
            _operands = operands;
            _size = size;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var expression = _operands[0].Evaluate(state);
            var result = expression.ZeroExtend(_size).Truncate(_size);

            state.Stack.SetVariable(Id, result);
        }
    }
}
