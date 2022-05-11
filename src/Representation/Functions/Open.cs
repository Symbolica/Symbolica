using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class Open : IFunction
{
    public Open(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var path = state.ReadString(exprFactory, arguments.Get(0));

        var descriptor = state.System.Open(path);

        state.Stack.SetVariable(caller.Id, exprFactory.CreateConstant(caller.Size, descriptor));
    }
}
