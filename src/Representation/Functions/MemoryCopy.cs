using System;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions;

internal sealed class MemoryCopy : IFunction
{
    public MemoryCopy(FunctionId id, IParameters parameters)
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

        var isInvalid = destination.NotEqual(source)
            .And(destination.UnsignedLess(source.Add(length))
                .And(source.UnsignedLess(destination.Add(length))));
        using var proposition = isInvalid.GetProposition(state.Space);

        if (proposition.CanBeTrue())
            throw new StateException(StateError.OverlappingMemoryCopy, proposition.CreateTrueSpace());

        state.ForkAll(length, new CopyMemory(destination, source));
    }

    private sealed class CopyMemory : IParameterizedStateAction
    {
        private readonly IExpression _destination;
        private readonly IExpression _source;

        public CopyMemory(IExpression destination, IExpression source)
        {
            _destination = destination;
            _source = source;
        }

        public void Invoke(IState state, BigInteger value)
        {
            var bytes = (Bytes) (uint) value;
            var sources = _source.GetAddresses(bytes);
            var destinations = _destination.GetAddresses(bytes);
            if (sources.Count() != destinations.Count())
                throw new Exception("Can't do a memory set when the source and destination have different field sizes.");

            foreach (var (source, destination) in sources.Zip(destinations))
            {
                if (source.Item2 != destination.Item2)
                    throw new Exception($"The source size '{source.Item2}' does not match the destination size '{destination.Item2}'.");
                state.Memory.Write(destination.Item1, state.Memory.Read(source.Item1, source.Item2.ToBits()));
            }
        }
    }
}
