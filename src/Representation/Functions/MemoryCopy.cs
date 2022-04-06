using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;

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

        var isInvalid = And.Create(
            And.Create(
                NotEqual.Create(destination, source),
                UnsignedLess.Create(destination, Add.Create(source, length))),
            UnsignedLess.Create(source, Add.Create(destination, length)));

        using var proposition = state.Space.CreateProposition(isInvalid);

        if (proposition.CanBeTrue())
            throw new StateException(StateError.OverlappingMemoryCopy, proposition.CreateTrueSpace());

        state.ForkAll(length, new CopyMemory(destination, source));
    }

    private sealed class CopyMemory : IParameterizedStateAction
    {
        private readonly IExpression<IType> _destination;
        private readonly IExpression<IType> _source;

        public CopyMemory(IExpression<IType> destination, IExpression<IType> source)
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
