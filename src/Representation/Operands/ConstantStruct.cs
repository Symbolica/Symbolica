using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;

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
            var address = Address.CreateNull(state.Space.PointerSize).AppendOffsets(element.Offset);
            expression = state.Space.Write(expression, address, value);
        }

        return expression;
    }
}
