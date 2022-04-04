using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;

namespace Symbolica.Representation.Functions;

internal sealed class Free : IFunction
{
    public Free(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.GetAddress(0);

        state.Fork(address,
            new FreeMemory(address),
            new NoOp());
    }

    private sealed class FreeMemory : IStateAction
    {
        private readonly Address _address;

        public FreeMemory(Address address)
        {
            _address = address;
        }

        public void Invoke(IState state)
        {
            state.Memory.Free(_address);
        }
    }

    private sealed class NoOp : IStateAction
    {
        public void Invoke(IState state)
        {
        }
    }
}
