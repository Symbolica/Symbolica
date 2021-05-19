using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions
{
    public sealed class ExtractElement : IInstruction
    {
        private readonly ulong _elementCount;
        private readonly Bits _elementSize;
        private readonly IOperand[] _operands;

        public ExtractElement(InstructionId id, IOperand[] operands, Bits elementSize, ulong elementCount)
        {
            Id = id;
            _operands = operands;
            _elementSize = elementSize;
            _elementCount = elementCount;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            throw new NotImplementedException(
                $"Scalarizer pass should deal with this. ({this} {_operands.Length} {_elementSize} {_elementCount})");
        }
    }
}