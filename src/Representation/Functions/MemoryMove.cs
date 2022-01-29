using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class MemoryMove : IFunction
{
    public MemoryMove(FunctionId id, IParameters parameters)
    {
        Id = id;
        Parameters = parameters;
    }

    public FunctionId Id { get; }
    public IParameters Parameters { get; }

    public void Call(IState state, ICaller caller, IArguments arguments)
    {
        var destination = arguments.Get(0);
        var source = arguments.Get(1);
        var length = arguments.Get(2);

        state.ForkAll(length, new MoveMemory(destination, source));
    }

    private sealed class MoveMemory : IParameterizedStateAction
    {
        private readonly IExpression _destination;
        private readonly IExpression _source;

        public MoveMemory(IExpression destination, IExpression source)
        {
            _destination = destination;
            _source = source;
        }

        public void Invoke(IState state, BigInteger value)
        {
            var length = (Bytes) (uint) value;

            if (length != Bytes.Zero)
                state.Memory.Write(_destination, state.Memory.Read(_source, length.ToBits()));
        }
    }
}
