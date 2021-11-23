using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
    [Serializable]
    public sealed class ShiftLeft : IInstruction
    {
        private readonly IOperand[] _operands;

        public ShiftLeft(InstructionId id, IOperand[] operands)
        {
            Id = id;
            _operands = operands;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var left = _operands[0].Evaluate(state);
            var right = _operands[1].Evaluate(state);

            var isUndefined = right.UnsignedGreaterOrEqual(state.Space.CreateConstant(right.Size, (uint)left.Size));
            using var proposition = isUndefined.GetProposition(state.Space);

            if (proposition.CanBeTrue)
                throw new StateException(StateError.UndefinedShift, proposition.FalseSpace);

            var result = left.ShiftLeft(right);

            state.Stack.SetVariable(Id, result);
        }
    }
}
