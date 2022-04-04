using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Functions;

internal sealed class Allocate : IFunction
{
    public Allocate(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var size = arguments.Get(0);

        state.ForkAll(size, new AllocateMemory(caller));
    }

    private sealed class AllocateMemory : IParameterizedStateAction
    {
        private readonly ICaller _caller;

        public AllocateMemory(ICaller caller)
        {
            _caller = caller;
        }

        public void Invoke(IState state, BigInteger value)
        {
            var size = (Bytes) (uint) value;

            var address = size == Bytes.Zero
                ? Address.CreateNull(state.Space.PointerSize)
                : state.Memory.Allocate(size.ToBits());

            state.Stack.SetVariable(_caller.Id, address);
        }
    }
}
