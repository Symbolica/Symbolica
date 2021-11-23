using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    [Serializable]
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

            state.ForkAll(length, new StateActions.CopyMemoryOfLength(destination, source));
        }
    }
}
