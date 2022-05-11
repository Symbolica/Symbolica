using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class GetPathStatus : IFunction
{
    public GetPathStatus(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var path = state.ReadString(exprFactory, arguments.Get(0));
        var address = arguments.Get(1);

        var descriptor = state.System.Open(path);
        var result = state.System.GetStatus(state.Space, state.Memory, descriptor, address);
        state.System.Close(descriptor);

        state.Stack.SetVariable(caller.Id, exprFactory.CreateConstant(caller.Size, result));
    }
}
