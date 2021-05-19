using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    internal sealed class VariadicCopy : IFunction
    {
        public VariadicCopy(FunctionId id, IParameters parameters)
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

            var size = state.Stack.GetInitializedVaList().Size;

            state.Memory.Write(destination, state.Memory.Read(source, size));
        }
    }
}