using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
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
            var address = arguments.Get(0);
            var size = arguments.Get(1);

            state.ForkAll(size, (s, v) => Call(s, caller, address, (Bytes) (uint) v));
        }

        private static void Call(IState state, ICaller caller, IExpression address, Bytes size)
        {
            if (size == Bytes.Zero)
                FreeMemory(state, caller, address);
            else
                AllocateMemory(state, caller, address, size.ToBits());
        }

        private static void FreeMemory(IState state, ICaller caller, IExpression address)
        {
            state.Memory.Free(address);
            state.Stack.SetVariable(caller.Id, state.Space.CreateConstant(address.Size, BigInteger.Zero));
        }

        private static void AllocateMemory(IState state, ICaller caller, IExpression address, Bits size)
        {
            state.Fork(address,
                s => s.Stack.SetVariable(caller.Id, s.Memory.Move(address, size)),
                s => s.Stack.SetVariable(caller.Id, s.Memory.Allocate(size)));
        }
    }
}
