using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class VariadicCopy : IFunction
{
    public VariadicCopy(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var destination = arguments.Get(0);
        var source = arguments.Get(1);

        var size = state.Stack.GetInitializedVaList().Size;

        state.Memory.Write(state.Space, destination, state.Memory.Read(state.Space, source, size));
    }
}
