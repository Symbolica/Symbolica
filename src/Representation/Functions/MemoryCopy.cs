using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
    internal sealed class MemoryCopy : IFunction
    {
        public MemoryCopy(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var destination = arguments.Get(0);
            var source = arguments.Get(1);
            var length = arguments.Get(2);

            var isInvalid = destination.NotEqual(source)
                .And(destination.UnsignedLess(source.Add(length))
                    .And(source.UnsignedLess(destination.Add(length))));
            using var proposition = isInvalid.GetProposition(state.Space);

            if (proposition.CanBeTrue)
                throw new StateException("Memory copy could overlap.", state.Space);

            state.ForkAll(length, (s, v) => Call(s, destination, source, (Bytes) (uint) v));
        }

        private static void Call(IState state, IExpression destination, IExpression source, Bytes length)
        {
            if (length != Bytes.Zero)
                state.Memory.Write(destination, state.Memory.Read(source, length.ToBits()));
        }
    }
}
