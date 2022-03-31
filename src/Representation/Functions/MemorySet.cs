using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

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
            IExpression MakeBuffer(Bytes size)
            {
                var bits = size.ToBits();
                var buffer = ConstantUnsigned.CreateZero(bits);
                foreach (var offset in Enumerable.Range(0, (int) (uint) size))
                    buffer.Write(state.Space, ConstantUnsigned.Create(bits, offset), _value);

                return buffer;
            }

            var addresses = Address.Create(_destination).GetAddresses((Bytes) (uint) bytes);

            foreach (var destination in addresses)
                state.Memory.Write(destination.Item1, MakeBuffer(destination.Item2));
        }
    }
}
