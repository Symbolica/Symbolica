using System.Collections.Generic;
using System.Linq;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.Instructions
{
    public sealed class Call : IInstruction, ICaller
    {
        private readonly IOperand[] _operands;
        private readonly IAttributes[] _parameterAttributes;

        public Call(InstructionId id, IOperand[] operands,
            Bits size, IAttributes attributes, IAttributes[] parameterAttributes)
        {
            Id = id;
            _operands = operands;
            Size = size;
            Attributes = attributes;
            _parameterAttributes = parameterAttributes;
        }

        public Bits Size { get; }
        public IAttributes Attributes { get; }

        public void Return(IState state)
        {
        }

        public InstructionId Id { get; }

        public void Execute(IState state)
        {
            var expressions = _operands.Select(o => o.Evaluate(state)).ToArray();

            var arguments = expressions.SkipLast(1).ToArray();
            var target = expressions.Last();

            state.ForkAll(target, (s, v) => Execute(s, (FunctionId) (ulong) v, arguments));
        }

        private void Execute(IState state, FunctionId target, IEnumerable<IExpression> arguments)
        {
            var function = state.GetFunction(target);

            function.Call(state, this, Coerce(function, arguments));
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