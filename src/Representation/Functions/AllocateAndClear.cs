using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
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

            state.ForkAll(size, (s, v) => Call(s, caller, (Bytes) (uint) v));
        }

        private static void Call(IState state, ICaller caller, Bytes size)
        {
            var address = size == Bytes.Zero
                ? state.Space.CreateConstant(state.Space.PointerSize, BigInteger.Zero)
                : AllocateMemory(state, size.ToBits());

            state.Stack.SetVariable(caller.Id, address);
        }

        private static IExpression AllocateMemory(IState state, Bits size)
        {
            var address = state.Memory.Allocate(size);
            state.Memory.Write(address, state.Space.CreateConstant(size, BigInteger.Zero));

            return address;
        }
    }
}