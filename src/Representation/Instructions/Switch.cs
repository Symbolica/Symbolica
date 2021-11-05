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

            new ExecuteSwitch(_operands, expression, 2).Run(state);
        }

        private class Transfer : IStateAction
        {
            private readonly IOperand[] _operands;
            private readonly IExpression _expression;
            private readonly int _index;

            public Transfer(IOperand[] operands, IExpression expression, int index)
            {
                _operands = operands;
                _expression = expression;
                _index = index;
            }

            public Unit Run(IState state)
            {
                var value = _operands[_index].Evaluate(state);
                var successorId = (BasicBlockId)(ulong)_operands[_index + 1].Evaluate(state).Constant;

                var isEqual = _expression.Equal(value);

                state.Fork(isEqual,
                    new TransferBasicBlock(successorId),
                    new ExecuteSwitch(_operands, _expression, _index + 2));
                return new Unit();
            }
        }

        private class ExecuteSwitch : IStateAction
        {
            private readonly IOperand[] _operands;
            private readonly IExpression _expression;
            private readonly int _index;

            public ExecuteSwitch(IOperand[] operands, IExpression expression, int index)
            {
                _operands = operands;
                _expression = expression;
                _index = index;
            }

            public Unit Run(IState state)
            {
                if (_index < _operands.Length)
                    new Transfer(_operands, _expression, _index).Run(state);
                else
                    new TransferBasicBlock((BasicBlockId)(ulong)_operands[1].Evaluate(state).Constant).Run(state);
                return new Unit();
            }
        }

        private class TransferBasicBlock : IStateAction
        {
            private readonly BasicBlockId _successorId;

            public TransferBasicBlock(BasicBlockId successorId)
            {
                _successorId = successorId;
            }

            public Unit Run(IState state)
            {
                state.Stack.TransferBasicBlock(_successorId);
                return new Unit();
            }
        }
    }
}
