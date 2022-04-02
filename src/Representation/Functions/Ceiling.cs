using Symbolica.Abstraction;
using Symbolica.Expression.Values;

namespace Symbolica.Representation.Functions;

internal sealed class Ceiling : IFunction
{
    public Ceiling(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var expression = arguments.Get(0);
        var result = FloatCeiling.Create(expression);

        state.Stack.SetVariable(caller.Id, result);
    }
}
