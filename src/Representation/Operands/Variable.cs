using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands
{
    public sealed class Variable : IOperand
    {
        private readonly InstructionId _instructionId;

        public Variable(InstructionId instructionId)
        {
            _instructionId = instructionId;
        }

        public IExpression Evaluate(IState state)
        {
            return Evaluate(state, false);
        }

        internal IExpression Evaluate(IState state, bool useIncomingValue)
        {
            return state.Stack.GetVariable(_instructionId, useIncomingValue);
        }
    }
}