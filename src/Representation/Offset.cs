using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Representation;

public sealed class Offset : IOperand
{
    private readonly IOperand _elementSize;
    private readonly IOperand _index;

    public Offset(IOperand elementSize, IOperand index)
    {
        _elementSize = elementSize;
        _index = index;
    }

    public IExpression<IType> Evaluate(IState state)
    {
        var count = _index.Evaluate(state);
        var size = _elementSize.Evaluate(state);

        return Multiply.Create(SignExtend.Create(state.Space.PointerSize, count), size);
    }
}
