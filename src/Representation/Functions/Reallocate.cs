using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Functions;

internal sealed class Reallocate : IFunction
{
    public Reallocate(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var address = arguments.GetAddress(0);
        var size = arguments.Get(1);

        state.ForkAll(size, new ReallocateMemory(caller, address));
    }

    private sealed class ReallocateMemory : IParameterizedStateAction
    {
        private readonly Address _address;
        private readonly ICaller _caller;

        public ReallocateMemory(ICaller caller, Address address)
        {
            _caller = caller;
            _address = address;
        }

        public void Invoke(IState state, BigInteger value)
        {
            var size = (Bytes) (uint) value;

            if (size == Bytes.Zero)
                Free(state, _caller, _address);
            else
                Allocate(state, _caller, _address, size.ToBits());
        }

        private static void Free(IState state, ICaller caller, Address address)
        {
            state.Memory.Free(address);
            state.Stack.SetVariable(caller.Id, Address.CreateNull(address.Type.Size));
        }

        private static void Allocate(IState state, ICaller caller, Address address, Bits size)
        {
            state.Fork(address,
                new MoveMemory(caller, address, size),
                new AllocateMemory(caller, size));
        }
    }

    private sealed class MoveMemory : IStateAction
    {
        private readonly Address _address;
        private readonly ICaller _caller;
        private readonly Bits _size;

        public MoveMemory(ICaller caller, Address address, Bits size)
        {
            _caller = caller;
            _address = address;
            _size = size;
        }

        public void Invoke(IState state)
        {
            state.Stack.SetVariable(_caller.Id, state.Memory.Move(_address, _size));
        }
    }

    private sealed class AllocateMemory : IStateAction
    {
        private readonly ICaller _caller;
        private readonly Bits _size;

        public AllocateMemory(ICaller caller, Bits size)
        {
            _caller = caller;
            _size = size;
        }

        public void Invoke(IState state)
        {
            state.Stack.SetVariable(_caller.Id, state.Memory.Allocate(_size));
        }
    }
}
