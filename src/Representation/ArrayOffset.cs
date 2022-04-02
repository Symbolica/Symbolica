using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

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

    public IExpression<IType> Evaluate(IState state)
    {
        var index = _index.Evaluate(state);
        var elementSize = ConstantUnsigned.Create(state.Space.PointerSize, (uint) _elementSize);

        return Multiply.Create(SignExtend.Create(state.Space.PointerSize, index), elementSize);
    }
}
