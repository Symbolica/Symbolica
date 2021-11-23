using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    [Serializable]
    internal sealed class ReadDirectory : IFunction
    {
        public ReadDirectory(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var address = arguments.Get(0);

            var result = state.System.ReadDirectory(address);

            state.Stack.SetVariable(caller.Id, result);
        }
    }
}
