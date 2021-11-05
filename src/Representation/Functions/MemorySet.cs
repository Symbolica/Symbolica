using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Functions
{
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

            state.ForkAll(length, new CallValueAction(destination, value));
        }

        private class CallValueAction : IForkAllAction
        {
            private readonly IExpression _destination;
            private readonly IExpression _value;

            public CallValueAction(IExpression destination, IExpression value)
            {
                _destination = destination;
                _value = value;
            }

            public IStateAction Run(BigInteger value) =>
                new CallAction(_destination, _value, (int)value);
        }

        private class CallAction : IStateAction
        {
            private readonly IExpression _destination;
            private readonly IExpression _value;
            private readonly int _length;

            public CallAction(IExpression destination, IExpression value, int length)
            {
                _destination = destination;
                _value = value;
                _length = length;
            }

            public Unit Run(IState state)
            {
                foreach (var offset in Enumerable.Range(0, _length))
                {
                    var address = _destination.Add(state.Space.CreateConstant(_destination.Size, offset));
                    state.Memory.Write(address, _value);
                }
                return new Unit();
            }
        }
    }
}
