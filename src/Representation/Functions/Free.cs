using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
    [Serializable]
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

            state.Fork(address, new StateActions.FreeMemory(address), new NoOp<IState>());
        }
    }
}
