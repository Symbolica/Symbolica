using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class Symbolize : IFunction
{
    public Symbolize(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.GetAddress(0);
        var size = (Bytes) (uint) state.Space.GetSingleValue(arguments.Get(1));
        var name = state.ReadString(arguments.GetAddress(2));

        state.Memory.Write(address, Symbol.Create(size.ToBits(), name));
    }
}
