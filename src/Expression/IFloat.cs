using System;

namespace Symbolica.Expression;

public interface IFloat : IExpression
{
    public static IExpression Unary(IExpression value,
        Func<float, IExpression> constantSingle,
        Func<double, IExpression> constantDouble,
        Func<IExpression, IExpression> symbolic)
    {
        return value is IConstantValue x
            ? (uint) x.Size switch
            {
                32U => constantSingle(x.AsSingle()),
                64U => constantDouble(x.AsDouble()),
                _ => symbolic(x)
            }
            : symbolic(value);
    }

    public static IExpression Binary(IExpression left, IExpression right,
        Func<float, float, IExpression> constantSingle,
        Func<double, double, IExpression> constantDouble,
        Func<IExpression, IExpression, IExpression> symbolic)
    {
        return left is IConstantValue x && right is IConstantValue y
            ? (uint) x.Size switch
            {
                32U => constantSingle(x.AsSingle(), y.AsSingle()),
                64U => constantDouble(x.AsDouble(), y.AsDouble()),
                _ => symbolic(x, y)
            }
            : symbolic(left, right);
    }
}
