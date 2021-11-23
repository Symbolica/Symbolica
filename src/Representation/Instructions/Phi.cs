using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Representation.Exceptions;
using Symbolica.Representation.Operands;

namespace Symbolica.Representation.Instructions
{
    [Serializable]
    public sealed class Phi : IInstruction
    {
        private readonly IReadOnlyDictionary<BasicBlockId, int> _indices;
        private readonly IOperand[] _operands;

        private Phi(InstructionId id, IOperand[] operands, IReadOnlyDictionary<BasicBlockId, int> indices)
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
                : throw new MissingBasicBlockException(state.Stack.PredecessorId);

            var result = Evaluate(state, _operands[index]);

            state.Stack.SetVariable(Id, result);
        }

        private static IExpression Evaluate(IState state, IOperand operand)
        {
            return operand is Variable variable
                ? variable.Evaluate(state, true)
                : operand.Evaluate(state);
        }

        public static IInstruction Create(InstructionId id, IOperand[] operands, IEnumerable<BasicBlockId> predecessors)
        {
            return new Phi(
                id,
                operands,
                predecessors
                    .Select((p, i) => new { p, i })
                    .ToLookup(p => p.p, p => p.i)
                    .ToDictionary(g => g.Key, g => g.First()));
        }
    }
}
