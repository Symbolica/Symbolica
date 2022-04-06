using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

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

    public IExpression<IType> Evaluate(IState state)
    {
        var sequence = ConstantUnsigned.CreateZero(_size) as IExpression<IType>;
        var offset = Bits.Zero;

        foreach (var element in _elements)
        {
            var value = element.Evaluate(state);
            sequence = state.Space.Write(sequence, ConstantUnsigned.Create(_size, (uint) offset), value);
            offset += value.Size;
        }

        return sequence;
    }
}
