using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class ConstantSequence : IOperand
{
    private readonly IOperand[] _elements;
    private readonly Size _size;

    public ConstantSequence(Size size, IOperand[] elements)
    {
        _size = size;
        _elements = elements;
    }

    public IExpression Evaluate(IState state)
    {
        var sequence = state.Space.CreateZero(_size);
        var offset = Size.Zero;

        foreach (var element in _elements)
        {
            var value = element.Evaluate(state);
            sequence = sequence.Write(state.Space.CreateConstant(_size, offset.Bits), value);
            offset += value.Size;
        }

        return sequence;
    }
}
