using Symbolica.Abstraction;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Functions;

internal sealed class IsTerminal : IFunction
{
    public IsTerminal(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        state.Stack.SetVariable(caller.Id, ConstantUnsigned.CreateZero(caller.Size));
    }
}
