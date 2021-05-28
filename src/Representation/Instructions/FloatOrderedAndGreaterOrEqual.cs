using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
    public sealed class FloatOrderedAndGreaterOrEqual : IInstruction
    {
        private readonly IOperand[] _operands;

        public FloatOrderedAndGreaterOrEqual(InstructionId id, IOperand[] operands)
        {
            Id = id;
            _operands = operands;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var left = _operands[0].Evaluate(state);
            var right = _operands[1].Evaluate(state);
            var result = left.FloatOrdered(right).And(left.FloatGreaterOrEqual(right));

            state.Stack.SetVariable(Id, result);
        }
    }
}
