using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions;

internal sealed class GetWorkingDirectory : IFunction
{
    public GetWorkingDirectory(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.GetAddress(0);

        state.Stack.SetVariable(caller.Id, address);
    }
}
