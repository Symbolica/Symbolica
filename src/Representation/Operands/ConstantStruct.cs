using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class ConstantStruct : IOperand
{
    private readonly StructElement[] _elements;
    private readonly Size _size;

    public ConstantStruct(Size size, StructElement[] elements)
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
            var offset = state.Space.CreateConstant(_size, element.Offset.Bits);
            expression = expression.Write(offset, value);
        }

        return expression;
    }
}
