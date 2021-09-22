using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    internal static class StateExtensions
    {
        public static void ForkAll(this IState self, IExpression expression, Action<IState, BigInteger> action)
        {
            var value = expression.GetValue(self.Space);
            var isEqual = expression.Equal(value);

            self.Fork(isEqual,
                s => action(s, value.Constant),
                s => ForkAll(s, expression, action));
        }

        public static string ReadString(this IState self, IExpression address)
        {
            return new(ReadCharacters(self, address).ToArray());
        }

        private static IEnumerable<char> ReadCharacters(IState state, IExpression address)
        {
            while (true)
            {
                var character = (char) state.Memory.Read(address, Bytes.One.ToBits()).Constant;
                if (character == default)
                    yield break;

                yield return character;
                address = address.Add(state.Space.CreateConstant(address.Size, (uint) Bytes.One));
            }
        }
    }
}
