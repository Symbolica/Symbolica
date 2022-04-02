using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation;

public sealed class StructOffset : IOperand
{
    private readonly Bytes _offset;

    public StructOffset(Bytes offset)
    {
        _offset = offset;
    }

    public IExpression<IType> Evaluate(IState state)
    {
        return ConstantUnsigned.Create(state.Space.PointerSize, (uint) _offset);
    }
}
