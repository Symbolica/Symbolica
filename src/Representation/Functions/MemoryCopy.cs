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

    public void Call(IExpressionFactory exprFactory, IState state, ICaller caller, IArguments arguments)
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

        state.ForkAll(exprFactory, length, new CopyMemory(destination, source));
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
            var destination = _destination is IAddress d ? d.AddImplicitOffsets() : _destination;
            var source = _source is IAddress s ? s.AddImplicitOffsets() : _source;
            var length = (Bytes) (uint) value;

            if (length != Bytes.Zero)
                state.Memory.Write(
                    state.Space,
                    destination,
                    state.Memory.Read(state.Space, source, length.ToBits()));
        }
    }
}
