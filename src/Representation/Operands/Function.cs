using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Operands;

public sealed class Function : IOperand
{
    private readonly FunctionId _id;

    public Function(FunctionId id)
    {
        _id = id;
    }

    public IExpression<IType> Evaluate(IState state)
    {
        return ConstantUnsigned.Create(state.Space.PointerSize, (ulong) _id);
    }
}
