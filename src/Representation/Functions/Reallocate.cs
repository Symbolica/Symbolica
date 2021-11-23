using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    [Serializable]
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

            state.ForkAll(size, new StateActions.ReallocateMemoryOfSize(caller.Id, address));
        }
    }
}
