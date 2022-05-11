using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class ReadDirectory : IFunction
{
    public ReadDirectory(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.Get(0);

        var result = state.System.ReadDirectory(state.Space, state.Memory, address);

        state.Stack.SetVariable(caller.Id, result);
    }
}
