using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

internal static class StateExtensions
{
    public static string ReadString(this IState self, IExpression address)
    {
        return new string(ReadCharacters(self, address).ToArray());
    }

    private static IEnumerable<char> ReadCharacters(IState state, IExpression address)
    {
        while (true)
        {
            var character = (char) state.Memory.Read(address, Bytes.One.ToBits()).GetSingleValue(state.Space);
            if (character == default)
                yield break;

            yield return character;
            address = address.OffsetBy(Bytes.One);
        }
    }

    public static void ForkAll(this IState self, IExpression expression, IParameterizedStateAction action)
    {
        var value = expression.GetExampleValue(self.Space);
        var isEqual = expression.Equal(self.Space.CreateConstant(expression.Size, value));

        self.Fork(isEqual,
            new Action(action, value),
            new Fork(action, expression));
    }

    private sealed class Action : IStateAction
    {
        private readonly IParameterizedStateAction _action;
        private readonly BigInteger _value;

        public Action(IParameterizedStateAction action, BigInteger value)
        {
            _action = action;
            _value = value;
        }

        public void Invoke(IState state)
        {
            _action.Invoke(state, _value);
        }
    }

    private sealed class Fork : IStateAction
    {
        private readonly IParameterizedStateAction _action;
        private readonly IExpression _expression;

        public Fork(IParameterizedStateAction action, IExpression expression)
        {
            _action = action;
            _expression = expression;
        }

        public void Invoke(IState state)
        {
            state.ForkAll(_expression, _action);
        }
    }
}
