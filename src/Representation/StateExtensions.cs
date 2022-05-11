using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;

namespace Symbolica.Representation;

internal static class StateExtensions
{
    public static string ReadString(this IState self, IExpressionFactory exprFactory, IExpression address)
    {
        return new string(ReadCharacters(self, exprFactory, address).ToArray());
    }

    private static IEnumerable<char> ReadCharacters(IState state, IExpressionFactory exprFactory, IExpression address)
    {
        while (true)
        {
            var character = (char) state.Memory.Read(address, Bytes.One.ToBits()).GetSingleValue(state.Space);
            if (character == default)
                yield break;

            yield return character;
            address = address.Add(exprFactory.CreateConstant(address.Size, (uint) Bytes.One));
        }
    }

    public static void ForkAll(this IState self, IExpressionFactory exprFactory, IExpression expression, IParameterizedStateAction action)
    {
        var value = expression.GetExampleValue(self.Space);
        var isEqual = expression.Equal(exprFactory.CreateConstant(expression.Size, value));

        self.Fork(isEqual,
            new Action(action, value),
            new Fork(exprFactory, action, expression));
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
        private readonly IExpressionFactory _exprFactory;
        private readonly IParameterizedStateAction _action;
        private readonly IExpression _expression;

        public Fork(IExpressionFactory exprFactory, IParameterizedStateAction action, IExpression expression)
        {
            _exprFactory = exprFactory;
            _action = action;
            _expression = expression;
        }

        public void Invoke(IState state)
        {
            state.ForkAll(_exprFactory, _expression, _action);
        }
    }
}
