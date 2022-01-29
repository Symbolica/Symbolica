using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions;

internal sealed class VariadicStart : IFunction
{
    public VariadicStart(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.Get(0);

        state.Memory.Write(address, state.Stack.GetInitializedVaList());
    }
}
