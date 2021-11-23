using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands
{
    [Serializable]
    public sealed class ConstantStruct : IOperand
    {
        private readonly StructElement[] _elements;
        private readonly Bits _size;

        public ConstantStruct(Bits size, StructElement[] elements)
        {
            _size = size;
            _elements = elements;
        }

        public IExpression Evaluate(IState state)
        {
            var expression = state.Space.CreateGarbage(_size);

            foreach (var element in _elements)
            {
                var value = element.Operand.Evaluate(state);
                var offset = state.Space.CreateConstant(_size, (uint)element.Offset);
                expression = expression.Write(offset, value);
            }

            return expression;
        }
    }
}
