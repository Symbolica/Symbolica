using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Operands;

public sealed class ConstantNull : IOperand
{
    public IExpression<IType> Evaluate(IState state)
    {
        return ConstantUnsigned.CreateZero(state.Space.PointerSize);
    }
}
