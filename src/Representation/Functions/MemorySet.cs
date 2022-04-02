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
        private readonly IExpression<IType> _destination;
        private readonly IExpression<IType> _value;

        public SetMemory(IExpression<IType> destination, IExpression<IType> value)
        {
            _destination = destination;
            _value = value;
        }

        public void Invoke(IState state, BigInteger bytes)
        {
            IExpression<IType> MakeBuffer(Bytes bytes)
            {
                var size = bytes.ToBits();
                return Enumerable.Range(0, (int) (uint) bytes)
                    .Select(offset => ConstantUnsigned.Create(size, offset))
                    .Aggregate(
                        ConstantUnsigned.CreateZero(size) as IExpression<IType>,
                        (buffer, offset) => state.Space.Write(buffer, offset, _value));
            }

            foreach (var destination in Address.Create(_destination).GetAddresses((Bytes) (uint) bytes))
                state.Memory.Write(destination.Item1, MakeBuffer(destination.Item2));
        }
    }
}
