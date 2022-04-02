using System;
using Symbolica.Expression;

namespace Symbolica.Computation;

internal static class ExpressionExtensions
{
    internal static V Map<T, U, V>(
        this IBinaryExpression<T> expression,
        ITypeMapper<U> operandMapper,
        Func<U, U, V> createExpr)
        where T : IType
        where U : IDisposable
    {
        using var left = expression.Left.Map(operandMapper);
        using var right = expression.Right.Map(operandMapper);
        return createExpr(left, right);
    }

    internal static V Map<T, U, V>(
        this IUnary<T> expression,
        ITypeMapper<U> operandMapper,
        Func<U, V> createExpr)
        where T : IType
        where U : IDisposable
    {
        using var value = expression.Value.Map(operandMapper);
        return createExpr(value);
    }
}
