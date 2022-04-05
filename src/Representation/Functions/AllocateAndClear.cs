using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class AllocateAndClear : IFunction
{
    public AllocateAndClear(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var size = arguments.Get(0).Multiply(arguments.Get(1));

        state.ForkAll(size, new AllocateAndClearMemory(caller));
    }

    private sealed class AllocateAndClearMemory : IParameterizedStateAction
    {
        private readonly ICaller _caller;

        public AllocateAndClearMemory(ICaller caller)
        {
            _caller = caller;
        }

        public void Invoke(IState state, BigInteger value)
        {
            var size = Size.FromBytes(value);

            var address = size == Size.Zero
                ? state.Space.CreateZero(state.Space.PointerSize)
                : Allocate(state, size);

            state.Stack.SetVariable(_caller.Id, address);
        }

        private static IExpression Allocate(IState state, Size size)
        {
            var address = state.Memory.Allocate(size);
            state.Memory.Write(address, state.Space.CreateZero(size));

            return address;
        }
    }
}
