using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands
{
    public sealed class GlobalVariable : IOperand
    {
        private readonly GlobalId _globalId;

        public GlobalVariable(GlobalId globalId)
        {
            _globalId = globalId;
        }

        public IExpression Evaluate(IState state)
        {
            return state.GetGlobalAddress(_globalId);
        }
    }
}