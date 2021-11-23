using System;
using System.Numerics;
using Symbolica.Abstraction;

namespace Symbolica.Representation.Functions
{
    [Serializable]
    internal sealed class LongJump : IFunction
    {
        public LongJump(FunctionId id, IParameters parameters)
        {
            Id = id;
            Parameters = parameters;
        }

        public FunctionId Id { get; }
        public IParameters Parameters { get; }

        public void Call(IState state, ICaller caller, IArguments arguments)
        {
            var address = arguments.Get(0);
            var value = arguments.Get(1);

            var instructionId = state.Stack.Restore(address, true);
            var result = value.Equal(state.Space.CreateConstant(value.Size, BigInteger.Zero))
                .Select(
                    state.Space.CreateConstant(value.Size, BigInteger.One),
                    value);

            state.Stack.SetVariable(instructionId, result);
        }
    }
}
