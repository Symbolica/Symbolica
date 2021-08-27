using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    public sealed class ConstantOffset : IOperand
    {
        private readonly Bytes _size;

        public ConstantOffset(Bytes size)
        {
            _size = size;
        }

        public IExpression Evaluate(IState state)
        {
            return state.Space.CreateConstant(state.Space.PointerSize, (uint) _size);
        }
    }
}
