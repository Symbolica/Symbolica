using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    [Serializable]
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
                throw new StateException(StateError.OverlappingMemoryCopy, state.Space);

            state.ForkAll(length, new StateActions.CopyMemoryOfLength(destination, source));
        }
    }
}
