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

    public IExpression Evaluate(IExpressionFactory exprFactory, IState state)
    {
        var sequence = exprFactory.CreateZero(_size);
        var offset = Bits.Zero;

        foreach (var element in _elements)
        {
            var value = element.Evaluate(exprFactory, state);
            sequence = sequence.Write(state.Space, exprFactory.CreateConstant(_size, (uint) offset), value);
            offset += value.Size;
        }

        return sequence;
    }
}
