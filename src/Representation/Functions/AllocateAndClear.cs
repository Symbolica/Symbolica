using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation.Functions;

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
        var size = Expression.Values.Multiply.Create(arguments.Get(0), arguments.Get(1));

        state.ForkAll(size, new AllocateAndClearMemory(caller));
    }

    private sealed class AllocateAndClearMemory : IParameterizedStateAction
    {
        private readonly ICaller _caller;

        public AllocateAndClearMemory(ICaller caller)
        {
            _caller = caller;
        }

        public void Invoke(IState state, BigInteger value)
        {
            var size = (Bytes) (uint) value;

            var address = size == Bytes.Zero
                ? ConstantUnsigned.CreateZero(state.Space.PointerSize)
                : Allocate(state, size.ToBits());

            state.Stack.SetVariable(_caller.Id, address);
        }

        private static IExpression Allocate(IState state, Bits size)
        {
            var address = state.Memory.Allocate(size);
            state.Memory.Write(address, ConstantUnsigned.CreateZero(size));

            return address;
        }
    }
}
