using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands
{
    public sealed class ConstantFloat : IOperand
    {
        private readonly Bits _size;
        private readonly string _value;

        public ConstantFloat(Bits size, string value)
        {
            _size = size;
            _value = value;
        }

        public IExpression Evaluate(IState state)
        {
            return state.Space.CreateConstantFloat(_size, _value);
        }
    }
}
