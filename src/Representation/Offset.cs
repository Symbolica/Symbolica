using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    [Serializable]
    public sealed class Offset : IOperand
    {
        private readonly IOperand _elementSize;
        private readonly IOperand _index;

        public Offset(IOperand elementSize, IOperand index)
        {
            _elementSize = elementSize;
            _index = index;
        }

        public IExpression Evaluate(IState state)
        {
            var count = _index.Evaluate(state);
            var size = _elementSize.Evaluate(state);

            return count.SignExtend(state.Space.PointerSize).Multiply(size);
        }
    }
}
