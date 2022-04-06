using Symbolica.Abstraction;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Functions;

internal sealed class GcUnimplemented : IFunction
{
    public GcUnimplemented(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        state.Stack.SetVariable(caller.Id, ConstantUnsigned.Create(caller.Size, 3U));
    }
}
