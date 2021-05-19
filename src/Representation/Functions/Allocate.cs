using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
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

            state.ForkAll(size, (s, v) => Call(s, caller, (Bytes) (uint) v));
        }

        private static void Call(IState state, ICaller caller, Bytes size)
        {
            var address = size == Bytes.Zero
                ? state.Space.CreateConstant(state.Space.PointerSize, BigInteger.Zero)
                : state.Memory.Allocate(size.ToBits());

            state.Stack.SetVariable(caller.Id, address);
        }
    }
}