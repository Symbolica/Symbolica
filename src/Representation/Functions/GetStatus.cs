using Symbolica.Abstraction;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Functions;

internal sealed class GetStatus : IFunction
{
    public GetStatus(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var descriptor = (int) state.Space.GetSingleValue(arguments.Get(0));
        var address = arguments.GetAddress(1);

        var result = state.System.GetStatus(descriptor, address);

        state.Stack.SetVariable(caller.Id, ConstantUnsigned.Create(caller.Size, result));
    }
}
