using System;
using System.Numerics;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    [Serializable]
    internal sealed class SignalAction : IFunction
    {
        public SignalAction(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            state.Stack.SetVariable(caller.Id, state.Space.CreateConstant(caller.Size, BigInteger.Zero));
        }
    }
}
