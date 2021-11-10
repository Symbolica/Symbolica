using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions
{
    public sealed class Call : IInstruction, ICaller
    {
        private readonly IOperand[] _operands;
        private readonly IAttributes[] _parameterAttributes;

        public Call(
            InstructionId id,
            IOperand[] operands,
            Bits size,
            IAttributes returnAttributes,
            IAttributes[] parameterAttributes)
        {
            Id = id;
            _operands = operands;
            Size = size;
            ReturnAttributes = returnAttributes;
            _parameterAttributes = parameterAttributes;
        }

        public Bits Size { get; }
        public IAttributes ReturnAttributes { get; }

        public void Return(IState state)
        {
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var expressions = _operands.Select(o => o.Evaluate(state)).ToArray();

            var arguments = expressions.SkipLast(1).ToArray();
            var target = expressions.Last();

            state.ForkAll(target, new Dispatch(this, arguments));
        }

        private sealed class Dispatch : IPartialAction
        {
            private readonly IEnumerable<IExpression> _arguments;
            private readonly Call _call;

            public Dispatch(Call call, IEnumerable<IExpression> arguments)
            {
                _call = call;
                _arguments = arguments;
            }

            public void Invoke(IState state, BigInteger value)
            {
                var function = state.GetFunction((FunctionId) (ulong) value);

                function.Call(state, _call, Coerce(function));
            }

            private Arguments Coerce(IFunction function)
            {
                return new(_arguments
                    .Select((a, i) => Coerce(function.Parameters, a, i))
                    .ToArray());
            }

            private IExpression Coerce(IParameters parameters, IExpression argument, int index)
            {
                return index < parameters.Count
                    ? Coerce(parameters.Get(index).Size, argument, _call._parameterAttributes[index])
                    : argument;
            }

            private static IExpression Coerce(Bits size, IExpression expression, IAttributes attributes)
            {
                return attributes.IsSignExtended
                    ? expression.SignExtend(size)
                    : expression.ZeroExtend(size);
            }
        }
    }
}
