using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Operands;

public sealed class GlobalVariable : IOperand
{
    private readonly GlobalId _id;

    public GlobalVariable(GlobalId id)
    {
        _id = id;
    }

    public IExpression<IType> Evaluate(IState state)
    {
        return state.GetGlobalAddress(_id);
    }
}
