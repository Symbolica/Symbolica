using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions
{
    public sealed class InsertValue : IInstruction
    {
        private readonly Bits[] _constantOffsets;
        private readonly IOperand[] _operands;

        public InsertValue(InstructionId id, IOperand[] operands, Bits[] constantOffsets)
        {
            Id = id;
            _operands = operands;
            _constantOffsets = constantOffsets;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var aggregate = _operands[0].Evaluate(state);
            var value = _operands[1].Evaluate(state);
            var offset = state.Space.CreateConstant(aggregate.Size,
                (uint) _constantOffsets.Aggregate(Bits.Zero, (l, r) => l + r));
            var result = aggregate.Write(offset, value);

            state.Stack.SetVariable(Id, result);
        }
    }
}
