using System.Numerics;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record FloatToSigned : IBitVector, IUnaryExpr
{
    private FloatToSigned(Bits size, IExpression value)
    {
        Size = size;
        Value = value;
    }

    public Bits Size { get; }

    public IExpression Value { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as FloatToSigned);
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
            v => ConstantSigned.Create(size, (BigInteger) v),
            v => ConstantSigned.Create(size, (BigInteger) v),
            v => v is IRealValue r
                ? new RealToSigned(size, r)
                : new FloatToSigned(size, v));
    }
}
