using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    internal interface IForkAllAction : IFunc<BigInteger, IStateAction>
    {
    }

    internal static class StateExtensions
    {
        public static void ForkAll(this IState self, IExpression expression, IForkAllAction action) =>
            new ForkAllAction(expression, action).Run(self);

        private class ForkAllAction : IStateAction
        {
            private readonly IExpression _expression;
            private readonly IForkAllAction _action;

            public ForkAllAction(IExpression expression, IForkAllAction action)
            {
                _expression = expression;
                _action = action;
            }

            public Unit Run(IState state)
            {
                var value = _expression.GetValue(state.Space);

                state.Fork(_expression.Equal(value),
                    _action.Run(value.Constant),
                    new ForkAllAction(_expression, _action));
                return new Unit();
            }
        }

        public static string ReadString(this IState self, IExpression address)
        {
            return new(ReadCharacters(self, address).ToArray());
        }

        private static IEnumerable<char> ReadCharacters(IState state, IExpression address)
        {
            while (true)
            {
                var character = (char)state.Memory.Read(address, Bytes.One.ToBits()).Constant;
                if (character == default)
                    yield break;

                yield return character;
                address = address.Add(state.Space.CreateConstant(address.Size, (uint)Bytes.One));
            }
        }
    }
}
