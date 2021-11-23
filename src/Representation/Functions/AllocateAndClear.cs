using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
    [Serializable]
    internal sealed class AllocateAndClear : IFunction
    {
        public AllocateAndClear(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var size = arguments.Get(0).Multiply(arguments.Get(1));

            state.ForkAll(
                size,
                new StateActions.AllocateAndClearMemoryOfSize().Map(
                    new StateActions.SetVariableFromFunc(caller.Id)));
        }
    }
}
