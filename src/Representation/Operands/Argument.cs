using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands
{
    public sealed class Argument : IOperand
    {
        private readonly int _index;

        public Argument(int index)
        {
            _index = index;
        }

        public IExpression Evaluate(IState state)
        {
            return state.Stack.GetFormal(_index);
        }
    }
}