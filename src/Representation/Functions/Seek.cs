using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    [Serializable]
    internal sealed class Seek : IFunction
    {
        public Seek(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var descriptor = (int) arguments.Get(0).Constant;
            var offset = (long) arguments.Get(1).Constant;
            var whence = (uint) arguments.Get(2).Constant;

            var result = state.System.Seek(descriptor, offset, whence);

            state.Stack.SetVariable(caller.Id, state.Space.CreateConstant(caller.Size, result));
        }
    }
}
