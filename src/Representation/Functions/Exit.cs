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
        var proposition = code.GetProposition(state.Space);

        if (proposition.CanBeTrue)
        {
            using var space = proposition.TrueSpace;

            throw new StateException(StateError.NonZeroExitCode, space.GetExample());
        }

        state.Complete();
    }
}
