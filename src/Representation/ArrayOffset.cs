using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

public sealed class ArrayOffset : IOperand
{
    private readonly Bytes _elementSize;
    private readonly IOperand _index;

    public ArrayOffset(Bytes elementSize, IOperand index)
    {
        _elementSize = elementSize;
        _index = index;
    }

    public IExpression Evaluate(IState state)
    {
        var index = _index.Evaluate(state);
        var elementSize = state.Space.CreateConstant(state.Space.PointerSize, (uint) _elementSize);

        return index.SignExtend(state.Space.PointerSize).Multiply(elementSize);
    }
}
