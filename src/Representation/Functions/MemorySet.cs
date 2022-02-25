using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class MemorySet : IFunction
{
    public MemorySet(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var destination = arguments.Get(0);
        var value = arguments.Get(1);
        var length = arguments.Get(2);

        state.ForkAll(length, new SetMemory(destination, value));
    }

    private sealed class SetMemory : IParameterizedStateAction
    {
        private readonly IExpression _destination;
        private readonly IExpression _value;

        public SetMemory(IExpression destination, IExpression value)
        {
            _destination = destination;
            _value = value;
        }

        public void Invoke(IState state, BigInteger bytes)
        {
            var size = ((Bytes) (uint) bytes).ToBits();
            var buffer = state.Space.CreateConstant(size, BigInteger.Zero);

            foreach (var offset in Enumerable.Range(0, (int) bytes))
                buffer.Write(state.Space, state.Space.CreateConstant(size, offset), _value);

            state.Memory.Write(_destination, buffer);
        }
    }
}
