using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions;

public sealed class Switch : IInstruction
{
    private readonly IOperand[] _operands;

    public Switch(InstructionId id, IOperand[] operands)
    {
        Id = id;
        _operands = operands;
    }

    public InstructionId Id { get; }

    public void Execute(IExpressionFactory exprFactory, IState state)
    {
        var expression = _operands[0].Evaluate(exprFactory, state);

        var action = new Transfer(exprFactory, expression, 2, _operands);
        action.Invoke(state);
    }

    private sealed class Transfer : IStateAction
    {
        private readonly IExpressionFactory _exprFactory;
        private readonly IExpression _expression;
        private readonly int _index;
        private readonly IOperand[] _operands;

        public Transfer(IExpressionFactory exprFactory, IExpression expression, int index, IOperand[] operands)
        {
            _exprFactory = exprFactory;
            _expression = expression;
            _index = index;
            _operands = operands;
        }

        public void Invoke(IState state)
        {
            if (_index < _operands.Length)
                TransferCase(state, _expression, _index);
            else
                TransferDefault(state);
        }

        private void TransferCase(IState state, IExpression expression, int index)
        {
            var value = _operands[index].Evaluate(_exprFactory, state);
            var successorId = (BasicBlockId) (ulong) _operands[index + 1].Evaluate(_exprFactory, state).GetSingleValue(state.Space);

            var isEqual = expression.Equal(value);

            state.Fork(isEqual,
                new TransferBasicBlock(successorId),
                new Transfer(_exprFactory, expression, index + 2, _operands));
        }

        private void TransferDefault(IState state)
        {
            var successorId = (BasicBlockId) (ulong) _operands[1].Evaluate(_exprFactory, state).GetSingleValue(state.Space);

            state.Stack.TransferBasicBlock(successorId);
        }
    }

    private sealed class TransferBasicBlock : IStateAction
    {
        private readonly BasicBlockId _id;

        public TransferBasicBlock(BasicBlockId id)
        {
            _id = id;
        }

        public void Invoke(IState state)
        {
            state.Stack.TransferBasicBlock(_id);
        }
    }
}
