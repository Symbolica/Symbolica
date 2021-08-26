using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    public sealed class ConstantOffset : IOperand
    {
        private readonly Bytes _offset;

        public ConstantOffset(Bytes offset)
        {
            _offset = offset;
        }

        public IExpression Evaluate(IState state)
        {
            return state.Space.CreateConstant(state.Space.PointerSize, (uint) _offset);
        }
    }
}
