using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class Allocate : IFunction
{
    public Allocate(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
    {
        var size = arguments.Get(0);

        state.ForkAll(exprFactory, size, new AllocateMemory(exprFactory, caller));
    }

    private sealed class AllocateMemory : IParameterizedStateAction
    {
        private readonly IExpressionFactory _exprFactory;
        private readonly ICaller _caller;

        public AllocateMemory(IExpressionFactory exprFactory, ICaller caller)
        {
            _exprFactory = exprFactory;
            _caller = caller;
        }

        public void Invoke(IState state, BigInteger value)
        {
            var size = (Bytes) (uint) value;

            var address = size == Bytes.Zero
                ? _exprFactory.CreateZero(_exprFactory.PointerSize)
                : state.Memory.Allocate(size.ToBits());

            state.Stack.SetVariable(_caller.Id, address);
        }
    }
}
