using System;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
    [Serializable]
    internal sealed class StackSave : IFunction
    {
        public StackSave(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var address = state.Stack.Allocate(Bytes.One.ToBits());

            state.Stack.Save(address, false);

            state.Stack.SetVariable(caller.Id, address);
        }
    }
}
