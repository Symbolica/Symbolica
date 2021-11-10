using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
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
            var address = arguments.Get(0);

            state.Fork(address,
                new FreeMemory(address),
                null);
        }

        private sealed class FreeMemory : IAction
        {
            private readonly IExpression _address;

            public FreeMemory(IExpression address)
            {
                _address = address;
            }

            public void Invoke(IState state)
            {
                state.Memory.Free(_address);
            }
        }
    }
}
