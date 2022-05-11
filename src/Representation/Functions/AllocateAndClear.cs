using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

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

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var size = arguments.Get(0).Multiply(arguments.Get(1));

        state.ForkAll(exprFactory, size, new AllocateAndClearMemory(exprFactory, caller));
    }

    private sealed class AllocateAndClearMemory : IParameterizedStateAction
    {
        private readonly IExpressionFactory _exprFactory;
        private readonly ICaller _caller;

        public AllocateAndClearMemory(IExpressionFactory exprFactory, ICaller caller)
        {
            _exprFactory = exprFactory;
            _caller = caller;
        }

        public void Invoke(IState state, BigInteger value)
        {
            var size = (Bytes) (uint) value;

            var address = size == Bytes.Zero
                ? _exprFactory.CreateZero(_exprFactory.PointerSize)
                : Allocate(state, size.ToBits());

            state.Stack.SetVariable(_caller.Id, address);
        }

        private IExpression Allocate(IState state, Bits size)
        {
            var address = state.Memory.Allocate(size);
            state.Memory.Write(state.Space, address, _exprFactory.CreateZero(size));

            return address;
        }
    }
}
