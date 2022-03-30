using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class ConstantSequence : IOperand
{
    private readonly IOperand[] _elements;
    private readonly Bits _size;

    public ConstantSequence(Bits size, IOperand[] elements)
    {
        _size = size;
        _elements = elements;
    }

    public IExpression Evaluate(IState state)
    {
        var sequence = state.Space.CreateZero(_size);
        var offset = Bits.Zero;

        foreach (var element in _elements)
        {
            var value = element.Evaluate(state);
            sequence = sequence.Write(state.Space.CreateConstant(_size, (uint) offset), value);
            offset += value.Size;
        }

        return sequence;
    }
}
