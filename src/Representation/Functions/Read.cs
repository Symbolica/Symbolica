using Symbolica.Abstraction;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Functions;

internal sealed class Read : IFunction
{
    public Read(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var descriptor = (int) state.Space.GetSingleValue(arguments.Get(0));
        var address = arguments.Get(1);
        var count = (int) state.Space.GetSingleValue(arguments.Get(2));

        var result = state.System.Read(descriptor, address, count);

        state.Stack.SetVariable(caller.Id, ConstantUnsigned.Create(caller.Size, result));
    }
}
