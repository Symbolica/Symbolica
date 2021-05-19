using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions
{
    public sealed class Allocate : IInstruction
    {
        private readonly Bits _elementSize;
        private readonly IOperand[] _operands;

        public Allocate(InstructionId id, IOperand[] operands, Bits elementSize)
        {
            Id = id;
            _operands = operands;
            _elementSize = elementSize;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var size = (Bits) ((uint) _elementSize * (uint) _operands[0].Evaluate(state).Integer);
            var address = state.Stack.Allocate(size);

            state.Stack.SetVariable(Id, address);
        }
    }
}