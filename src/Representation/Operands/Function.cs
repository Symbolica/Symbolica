using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands
{
    public sealed class Function : IOperand
    {
        private readonly FunctionId _functionId;

        public Function(FunctionId functionId)
        {
            _functionId = functionId;
        }

        public IExpression Evaluate(IState state)
        {
            return state.Space.CreateConstant(state.Space.PointerSize, (ulong) _functionId);
        }
    }
}