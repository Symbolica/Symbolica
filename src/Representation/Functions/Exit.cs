using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions;

internal sealed class Exit : IFunction
{
    public Exit(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var code = arguments.Get(0);
        using var proposition = code.GetProposition(state.Space);

        if (proposition.CanBeTrue())
            throw new StateException(StateError.NonZeroExitCode, proposition.CreateTrueSpace());

        state.Complete();
    }
}
