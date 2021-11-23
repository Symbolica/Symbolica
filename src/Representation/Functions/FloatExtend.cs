using System;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    [Serializable]
    internal sealed class FloatExtend : IFunction
    {
        public FloatExtend(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var expression = arguments.Get(0);
            var result = expression.FloatConvert(caller.Size);

            state.Stack.SetVariable(caller.Id, result);
        }
    }
}
