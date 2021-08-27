using System;
using System.Collections.Generic;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Operands;

namespace Symbolica.Representation.Instructions
{
    public sealed class Phi : IInstruction
    {
        private readonly IReadOnlyDictionary<BasicBlockId, int> _indices;
        private readonly IOperand[] _operands;

        public Phi(InstructionId id, IOperand[] operands, IReadOnlyDictionary<BasicBlockId, int> indices)
        {
            Id = id;
            _operands = operands;
            _indices = indices;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var index = _indices.TryGetValue(state.Stack.PredecessorId, out var value)
                ? value
                : throw new Exception($"Basic block {state.Stack.PredecessorId} was not found.");

            var result = Evaluate(state, _operands[index]);

            state.Stack.SetVariable(Id, result);
        }

        private static IExpression Evaluate(IState state, IOperand operand)
        {
            return operand is Variable variable
                ? variable.Evaluate(state, true)
                : operand.Evaluate(state);
        }
    }
}
