using System;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;

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
        private readonly IExpression<IType> _destination;
        private readonly IExpression<IType> _source;

        public MoveMemory(IExpression<IType> destination, IExpression<IType> source)
        {
            _destination = destination;
            _source = source;
        }

        public void Invoke(IState state, BigInteger value)
        {
            var bytes = (Bytes) (uint) value;
            var sources = Address.Create(_source).GetAddresses(bytes);
            var destinations = Address.Create(_destination).GetAddresses(bytes);
            if (sources.Count() != destinations.Count())
                throw new Exception("Can't do a memory move when the source and destination have different field sizes.");

            foreach (var (source, destination) in sources.Zip(destinations))
            {
                if (source.Item2 != destination.Item2)
                    throw new Exception($"The source size '{source.Item2}' does not match the destination size '{destination.Item2}'.");
                state.Memory.Write(destination.Item1, state.Memory.Read(source.Item1, source.Item2.ToBits()));
            }
        }
    }
}
