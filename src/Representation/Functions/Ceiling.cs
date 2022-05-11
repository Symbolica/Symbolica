using Symbolica.Abstraction;
using Symbolica.Expression;

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

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var expression = arguments.Get(0);
        var result = expression.FloatCeiling();

        state.Stack.SetVariable(caller.Id, result);
    }
}
