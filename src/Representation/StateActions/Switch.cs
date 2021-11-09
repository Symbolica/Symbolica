using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.StateActions
{
    public class Switch : IStateAction
    {
        private readonly IOperand[] _operands;
        private readonly IExpression _expression;
        private readonly int _index;

        private Switch(IOperand[] operands, IExpression expression, int index)
        {
            _operands = operands;
            _expression = expression;
            _index = index;
        }

        public static void Run(IOperand[] operands, IState state)
        {
            var expression = operands[0].Evaluate(state);
            new Switch(operands, expression, 2).Run(state);
        }

        public Unit Run(IState state)
        {
            if (_index < _operands.Length)
                Transfer(state);
            else
                state.Stack.TransferBasicBlock(GetSuccessorId(state, 1));
            return new Unit();
        }

        private BasicBlockId GetSuccessorId(IState state, int idx)
        {
            return (BasicBlockId)(ulong)_operands[idx].Evaluate(state).Constant;
        }

        private void Transfer(IState state)
        {
            var value = _operands[_index].Evaluate(state);
            var isEqual = _expression.Equal(value);

            state.Fork(isEqual,
                new StateActions.TransferBasicBlock(GetSuccessorId(state, _index + 1)),
                new Switch(_operands, _expression, _index + 2));
        }
    }
}
