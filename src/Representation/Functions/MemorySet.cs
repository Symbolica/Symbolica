using Symbolica.Abstraction;

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

            state.ForkAll(length, new StateActions.SetMemoryOfLength(destination, value));
        }
    }
}
