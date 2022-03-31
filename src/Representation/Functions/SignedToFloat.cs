using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions;

internal sealed class SignedToFloat : IFunction
{
    public SignedToFloat(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var expression = arguments.Get(0);
        var result = Expression.Values.SignedToFloat.Create(caller.Size, expression);

        state.Stack.SetVariable(caller.Id, result);
    }
}
