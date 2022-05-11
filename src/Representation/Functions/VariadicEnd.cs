using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class VariadicEnd : IFunction
{
    public VariadicEnd(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.Get(0);

        var size = state.Stack.GetInitializedVaList(state.Space).Size;

        state.Memory.Write(state.Space, address, exprFactory.CreateGarbage(size));
    }
}
