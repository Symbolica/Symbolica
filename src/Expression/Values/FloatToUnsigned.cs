using System.Numerics;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record FloatToUnsigned : IBitVector, IUnaryExpr
{
    private FloatToUnsigned(Bits size, IExpression value)
    {
        Size = size;
        Value = value;
    }

    public Bits Size { get; }

    public IExpression Value { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as FloatToUnsigned);
    }

    public T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IUnaryExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression Create(Bits size, IExpression value)
    {
        return IFloat.Unary(value,
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            v => ConstantUnsigned.Create(size, (BigInteger) v),
            v => new FloatToUnsigned(size, v));
    }
}
