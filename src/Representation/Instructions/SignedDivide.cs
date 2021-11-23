using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Instructions
{
    [Serializable]
    public sealed class SignedDivide : IInstruction
    {
        private readonly IOperand[] _operands;

        public SignedDivide(InstructionId id, IOperand[] operands)
        {
            Id = id;
            _operands = operands;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var left = _operands[0].Evaluate(state);
            var right = _operands[1].Evaluate(state);

            using var proposition = right.GetProposition(state.Space);

            if (proposition.CanBeFalse)
                throw new StateException(StateError.DivideByZero, proposition.FalseSpace);

            var result = left.SignedDivide(right);

            state.Stack.SetVariable(Id, result);
        }
    }
}
