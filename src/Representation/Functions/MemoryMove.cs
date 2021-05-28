using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
    internal sealed class MemoryMove : IFunction
    {
        public MemoryMove(FunctionId id, IParameters parameters)
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

            state.ForkAll(length, (s, v) => Call(s, destination, source, (Bytes) (uint) v));
        }

        private static void Call(IState state, IExpression destination, IExpression source, Bytes length)
        {
            if (length != Bytes.Zero)
                state.Memory.Write(destination, state.Memory.Read(source, length.ToBits()));
        }
    }
}
