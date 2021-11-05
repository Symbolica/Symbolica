using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
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

            if (proposition.CanBeTrue)
                throw new StateException(StateError.OverlappingMemoryCopy, state.Space);

            state.ForkAll(length, new CallAction(destination, source));
        }

        private class CallAction : IForkAllAction
        {
            private readonly IExpression _destination;
            private readonly IExpression _source;

            public CallAction(IExpression destination, IExpression source)
            {
                _destination = destination;
                _source = source;
            }

            public IStateAction Run(BigInteger value) =>
                new Write(_destination, _source, (Bytes)(uint)value);
        }

        private class Write : IStateAction
        {
            private readonly IExpression _source;
            private readonly IExpression _destination;
            private readonly Bytes _length;

            public Write(IExpression destination, IExpression source, Bytes length)
            {
                _source = source;
                _destination = destination;
                _length = length;
            }

            public Unit Run(IState state)
            {
                if (_length != Bytes.Zero)
                    state.Memory.Write(_destination, state.Memory.Read(_source, _length.ToBits()));
                return new Unit();
            }
        }
    }
}
