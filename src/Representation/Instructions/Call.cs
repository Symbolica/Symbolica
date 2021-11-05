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

            state.ForkAll(target, new CoerceValueAction(arguments, this, _parameterAttributes));
        }

        private class CoerceValueAction : IForkAllAction
        {
            private readonly IEnumerable<IExpression> _arguments;
            private readonly ICaller _caller;
            private readonly IAttributes[] _parameterAttributes;

            public CoerceValueAction(
                IEnumerable<IExpression> arguments,
                ICaller caller,
                IAttributes[] parameterAttributes)
            {
                _arguments = arguments;
                _caller = caller;
                _parameterAttributes = parameterAttributes;
            }

            public IStateAction Run(BigInteger value) =>
                new CoerceAction((FunctionId)(ulong)value, _arguments, _caller, _parameterAttributes);
        }

        private class CoerceAction : IStateAction
        {
            private readonly FunctionId _target;
            private readonly IEnumerable<IExpression> _arguments;
            private readonly ICaller _caller;
            private readonly IAttributes[] _parameterAttributes;

            public CoerceAction(
                FunctionId target,
                IEnumerable<IExpression> arguments,
                ICaller caller,
                IAttributes[] parameterAttributes)
            {
                _target = target;
                _arguments = arguments;
                _caller = caller;
                _parameterAttributes = parameterAttributes;
            }

            public Unit Run(IState state)
            {
                var function = state.GetFunction(_target);
                function.Call(state, _caller, Coerce(function, _arguments));
                return new Unit();
            }

            private Arguments Coerce(IFunction function, IEnumerable<IExpression> arguments)
            {
                return new(arguments
                    .Select((a, i) => Coerce(function.Parameters, a, i))
                    .ToArray());
            }

            private IExpression Coerce(IParameters parameters, IExpression argument, int index)
            {
                return index < parameters.Count
                    ? Coerce(parameters.Get(index).Size, argument, _parameterAttributes[index])
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
