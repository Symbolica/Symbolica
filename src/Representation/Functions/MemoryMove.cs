using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
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

            state.ForkAll(length, new CallValueAction(destination, source));
        }

        private class CallValueAction : IForkAllAction
        {
            private readonly IExpression _destination;
            private readonly IExpression _source;

            public CallValueAction(IExpression destination, IExpression source)
            {
                _destination = destination;
                _source = source;
            }

            public IStateAction Run(BigInteger value) =>
                new CallAction(_destination, _source, (Bytes)(uint)value);
        }

        private class CallAction : IStateAction
        {
            private readonly IExpression _destination;
            private readonly IExpression _source;
            private readonly Bytes _length;

            public CallAction(IExpression destination, IExpression source, Bytes length)
            {
                _destination = destination;
                _source = source;
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
