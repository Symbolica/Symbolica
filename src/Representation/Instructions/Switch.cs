using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions
{
    public sealed class Switch : IInstruction
    {
        private readonly IOperand[] _operands;

        public Switch(InstructionId id, IOperand[] operands)
        {
            Id = id;
            _operands = operands;
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var expression = _operands[0].Evaluate(state);

            Execute(state, expression, 2);
        }

        private void Execute(IState state, IExpression expression, int index)
        {
            if (index < _operands.Length)
                Transfer(state, expression, index);
            else
                TransferDefault(state);
        }

        private void Transfer(IState state, IExpression expression, int index)
        {
            var value = _operands[index].Evaluate(state);
            var successorId = (BasicBlockId) (ulong) _operands[index + 1].Evaluate(state).Integer;

            var isEqual = expression.Equal(value);

            state.Fork(isEqual,
                s => s.Stack.TransferBasicBlock(successorId),
                s => Execute(s, expression, index + 2));
        }

        private void TransferDefault(IState state)
        {
            var successorId = (BasicBlockId) (ulong) _operands[1].Evaluate(state).Integer;

            state.Stack.TransferBasicBlock(successorId);
        }
    }
}
