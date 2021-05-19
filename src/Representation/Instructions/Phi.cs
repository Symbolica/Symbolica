using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Operands;

namespace Symbolica.Representation.Instructions
{
    public sealed class Phi : IInstruction
    {
        private readonly IReadOnlyDictionary<BasicBlockId, int> _indices;
        private readonly IOperand[] _operands;

        public Phi(InstructionId id, IOperand[] operands, IEnumerable<BasicBlockId> predecessorIds)
        {
            Id = id;
            _operands = operands;
            _indices = GetIndices(predecessorIds);
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var index = _indices[state.Stack.PredecessorId];
            var result = Evaluate(state, _operands[index]);

            state.Stack.SetVariable(Id, result);
        }

        private static IExpression Evaluate(IState state, IOperand operand)
        {
            return operand is Variable variable
                ? variable.Evaluate(state, true)
                : operand.Evaluate(state);
        }

        private static IReadOnlyDictionary<BasicBlockId, int> GetIndices(IEnumerable<BasicBlockId> predecessorIds)
        {
            var indices = new Dictionary<BasicBlockId, int>();

            foreach (var (predecessorId, index) in predecessorIds.Select((p, i) => (p, i)))
                indices.TryAdd(predecessorId, index);

            return indices;
        }
    }
}