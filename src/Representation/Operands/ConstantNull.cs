using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class ConstantNull : IOperand
{
    public IExpression Evaluate(IState state)
    {
        return state.Space.CreateZero(state.Space.PointerSize);
    }
}
