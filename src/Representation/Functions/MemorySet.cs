using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
    internal sealed class MemorySet : IFunction
    {
        public MemorySet(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var destination = arguments.Get(0);
            var value = arguments.Get(1);
            var length = arguments.Get(2);

            state.ForkAll(length, (s, v) => Call(s, destination, value, (int) v));
        }

        private static void Call(IState state, IExpression destination, IExpression value, int length)
        {
            foreach (var offset in Enumerable.Range(0, length))
            {
                var address = destination.Add(state.Space.CreateConstant(destination.Size, offset));
                state.Memory.Write(address, value);
            }
        }
    }
}
