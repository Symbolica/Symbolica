using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    [Serializable]
    internal sealed class VariadicEnd : IFunction
    {
        public VariadicEnd(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var address = arguments.Get(0);

            var size = state.Stack.GetInitializedVaList().Size;

            state.Memory.Write(address, state.Space.CreateGarbage(size));
        }
    }
}
