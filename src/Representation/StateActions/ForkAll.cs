using System;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation.StateActions
{
    [Serializable]
    public class ForkAll : IStateAction
    {
        private readonly IExpression _expression;
        private readonly IFunc<BigInteger, IStateAction> _action;

        public ForkAll(IExpression expression, IFunc<BigInteger, IStateAction> action)
        {
            _expression = expression;
            _action = action;
        }

        public Unit Run(IState state)
        {
            var value = _expression.GetValue(state.Space);

            state.Fork(_expression.Equal(value),
                _action.Run(value.Constant),
                new ForkAll(_expression, _action));
            return new Unit();
        }
    }
}
