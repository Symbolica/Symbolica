using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions
{
    public sealed class ExtractValue : IInstruction
    {
        private readonly Bits[] _constantOffsets;
        private readonly IOperand[] _operands;
        private readonly Bits _size;

        public ExtractValue(InstructionId id, IOperand[] operands, Bits size, Bits[] constantOffsets)
        {
            Id = id;
            _operands = operands;
            _size = size;
            _constantOffsets = constantOffsets;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var aggregate = _operands[0].Evaluate(state);
            var offset = state.Space.CreateConstant(aggregate.Size,
                (uint) _constantOffsets.Aggregate(Bits.Zero, (l, r) => l + r));
            var result = aggregate.Read(offset, _size);

            state.Stack.SetVariable(Id, result);
        }
    }
}