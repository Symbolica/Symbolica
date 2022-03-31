using Symbolica.Abstraction;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Functions;

internal sealed class Close : IFunction
{
    public Close(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var descriptor = (int) state.Space.GetSingleValue(arguments.Get(0));

        var result = state.System.Close(descriptor);

        state.Stack.SetVariable(caller.Id, ConstantUnsigned.Create(caller.Size, result));
    }
}
