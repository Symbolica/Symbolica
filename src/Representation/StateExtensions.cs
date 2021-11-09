using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation
{
    internal static class StateExtensions
    {
        public static void ForkAll(this IState self, IExpression expression, IFunc<BigInteger, IStateAction> action) =>
            new StateActions.ForkAll(expression, action).Run(self);

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
