using Symbolica.Expression;

namespace Symbolica.Abstraction;

public interface IFunction
{
    FunctionId Id { get; }
    IParameters Parameters { get; }

    void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments);
}
