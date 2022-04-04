using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Abstraction;

public interface IState
{
    ISpace Space { get; }
    IMemory Memory { get; }
    IStack Stack { get; }
    ISystem System { get; }

    IFunction GetFunction(FunctionId id);
    Address GetGlobalAddress(GlobalId id);
    void Complete();
    void Fork(IExpression<IType> condition, IStateAction trueAction, IStateAction falseAction);
}
