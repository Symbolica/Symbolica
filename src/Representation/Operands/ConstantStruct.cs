using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Operands;

public sealed class ConstantStruct : IOperand
{
    private readonly StructElement[] _elements;
    private readonly Bits _size;

    public ConstantStruct(Bits size, StructElement[] elements)
    {
        _size = size;
        _elements = elements;
    }

    public IExpression<IType> Evaluate(IState state)
    {
        var expression = state.Space.CreateGarbage(_size);

        foreach (var element in _elements)
        {
            var value = element.Operand.Evaluate(state);
            var offset = ConstantUnsigned.Create(_size, (uint) element.Offset);
            expression = state.Space.Write(expression, offset, value);
        }

        return expression;
    }
}
