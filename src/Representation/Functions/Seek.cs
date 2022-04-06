using Symbolica.Abstraction;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Functions;

internal sealed class Seek : IFunction
{
    public Seek(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var descriptor = (int) state.Space.GetSingleValue(arguments.Get(0));
        var offset = (long) state.Space.GetSingleValue(arguments.Get(1));
        var whence = (uint) state.Space.GetSingleValue(arguments.Get(2));

        var result = state.System.Seek(descriptor, offset, whence);

        state.Stack.SetVariable(caller.Id, ConstantUnsigned.Create(caller.Size, result));
    }
}
