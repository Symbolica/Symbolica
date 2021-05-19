using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions
{
    public sealed class GetElementPointer : IInstruction
    {
        private readonly Bytes[] _constantOffsets;
        private readonly Offset[] _offsets;
        private readonly IOperand[] _operands;

        public GetElementPointer(InstructionId id, IOperand[] operands,
            Bytes[] constantOffsets, Offset[] offsets)
        {
            Id = id;
            _operands = operands;
            _constantOffsets = constantOffsets;
            _offsets = offsets;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var address = _operands[0].Evaluate(state);
            var constantOffset = state.Space.CreateConstant(state.Space.PointerSize,
                (uint) _constantOffsets.Aggregate(Bytes.Zero, (l, r) => l + r));
            var offset = _offsets.Aggregate(constantOffset, (l, r) => l.Add(CreateOffset(state, r)));
            var result = address.Add(offset);

            state.Stack.SetVariable(Id, result);
        }

        private IExpression CreateOffset(IState state, Offset offset)
        {
            var elementCount = _operands[offset.OperandNumber].Evaluate(state);

            return elementCount.SignExtend(state.Space.PointerSize)
                .Multiply(state.Space.CreateConstant(state.Space.PointerSize, (uint) offset.ElementSize));
        }
    }
}