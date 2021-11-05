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

            state.Fork(address, new FreeMemory(address), new NoOp());
        }

        private class FreeMemory : IStateAction
        {
            private readonly IExpression _address;

            public FreeMemory(IExpression address)
            {
                _address = address;
            }

            public Unit Run(IState state)
            {
                state.Memory.Free(_address);
                return new Unit();
            }
        }

        private class NoOp : IStateAction
        {
            public Unit Run(IState state) => new Unit();
        }
    }
}
