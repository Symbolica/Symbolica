using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    [Serializable]
    internal sealed class FloatRemainder : IFunction
    {
        public FloatRemainder(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var left = arguments.Get(0);
            var right = arguments.Get(1);
            var result = left.FloatRemainder(right);

            state.Stack.SetVariable(caller.Id, result);
        }
    }
}
