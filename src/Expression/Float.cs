using System;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression;

public sealed record Float : IType
{
    public Float(Bits size)
    {
        Size = size;
    }

    public Bits Size { get; }

    public static Float Single() => new((Bits) 32U);

    public static Float Double() => new((Bits) 64U);

    public static IExpression<IType> CreateConstant(Bits size, string value)
    {
        return (uint) size switch
        {
            32U => new ConstantSingle(float.Parse(value)),
            64U => new ConstantDouble(double.Parse(value)),
            _ => new NormalFloat(size, value)
        };
    }

    internal static IExpression<IType> Unary(IExpression<IType> value,
        Func<float, IExpression<IType>> constantSingle,
        Func<double, IExpression<IType>> constantDouble,
        Func<IExpression<IType>, IExpression<IType>> symbolic)
    {
        return value is IConstantValue<IType> x
            ? (uint) x.Size switch
            {
                32U => constantSingle(x.AsSingle()),
                64U => constantDouble(x.AsDouble()),
                _ => symbolic(x)
            }
            : symbolic(value);
    }

    internal static IExpression<IType> Binary(IExpression<IType> left, IExpression<IType> right,
        Func<float, float, IExpression<IType>> constantSingle,
        Func<double, double, IExpression<IType>> constantDouble,
        Func<IExpression<IType>, IExpression<IType>, IExpression<IType>> symbolic)
    {
        return left is IConstantValue<IType> x && right is IConstantValue<IType> y
            ? (uint) x.Size switch
            {
                32U => constantSingle(x.AsSingle(), y.AsSingle()),
                64U => constantDouble(x.AsDouble(), y.AsDouble()),
                _ => symbolic(x, y)
            }
            : symbolic(left, right);
    }
}
