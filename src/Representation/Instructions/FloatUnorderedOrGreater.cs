using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
    public sealed class FloatUnorderedOrGreater : IInstruction
    {
        private readonly IOperand[] _operands;

        public FloatUnorderedOrGreater(InstructionId id, IOperand[] operands)
        {
            Id = id;
            _operands = operands;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var left = _operands[0].Evaluate(state);
            var right = _operands[1].Evaluate(state);
            var result = left.FloatUnordered(right).Or(left.FloatGreater(right));

            state.Stack.SetVariable(Id, result);
        }
    }
}
