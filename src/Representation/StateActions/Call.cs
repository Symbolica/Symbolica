using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.StateActions
{
    [Serializable]
    public class Call : IStateAction
    {
        private readonly FunctionId _target;
        private readonly IEnumerable<IExpression> _arguments;
        private readonly ICaller _caller;
        private readonly IAttributes[] _parameterAttributes;

        public Call(
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

    [Serializable]
    public class CallTarget : IFunc<BigInteger, IStateAction>
    {
        private readonly IEnumerable<IExpression> _arguments;
        private readonly ICaller _caller;
        private readonly IAttributes[] _parameterAttributes;

        public CallTarget(
            IEnumerable<IExpression> arguments,
            ICaller caller,
            IAttributes[] parameterAttributes)
        {
            _arguments = arguments;
            _caller = caller;
            _parameterAttributes = parameterAttributes;
        }

        public IStateAction Run(BigInteger value) =>
            new Call((FunctionId)(ulong)value, _arguments, _caller, _parameterAttributes);
    }
}
