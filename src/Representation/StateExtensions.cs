using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Symbolica.Abstraction;
using Symbolica.Expression;
using Symbolica.Expression.Values;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Representation;

internal static class StateExtensions
{
    public static string ReadString(this IState self, IExpression<IType> address)
    {
        return new string(ReadCharacters(self, address).ToArray());
    }

    private static IEnumerable<char> ReadCharacters(IState state, IExpression<IType> address)
    {
        while (true)
        {
            var character = (char) state.Space.GetSingleValue(state.Memory.Read(address, Bytes.One.ToBits()));
            if (character == default)
                yield break;

            yield return character;
            address = Add.Create(address, ConstantUnsigned.Create(address.Size, (uint) Bytes.One));
        }
    }

    public static void ForkAll(this IState self, IExpression<IType> expression, IParameterizedStateAction action)
    {
        var value = self.Space.GetExampleValue(expression);
        var isEqual = Equal.Create(expression, ConstantUnsigned.Create(expression.Size, value));

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
        private readonly IExpression<IType> _expression;

        public Fork(IParameterizedStateAction action, IExpression<IType> expression)
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
