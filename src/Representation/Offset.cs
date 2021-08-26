using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    public sealed class Offset : IOperand
    {
        private readonly Bytes _elementSize;
        private readonly IOperand _operand;

        public Offset(Bytes elementSize, IOperand operand)
        {
            _elementSize = elementSize;
            _operand = operand;
        }

        public IExpression Evaluate(IState state)
        {
            var elementCount = _operand.Evaluate(state);

            return elementCount.SignExtend(state.Space.PointerSize)
                .Multiply(state.Space.CreateConstant(state.Space.PointerSize, (uint) _elementSize));
        }
    }
}
