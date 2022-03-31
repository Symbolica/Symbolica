using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record FloatConvert : IUnaryExpr, IFloat
{
    private FloatConvert(Bits size, IExpression value)
    {
        Size = size;
        Value = value;
    }

    public Bits Size { get; }

    public IExpression Value { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as FloatConvert);
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
            v => (uint) size switch
            {
                32U => new ConstantSingle(v),
                64U => new ConstantDouble(v),
                _ => new FloatConvert(size, new ConstantSingle(v))
            },
            v => (uint) size switch
            {
                32U => new ConstantSingle((float) v),
                64U => new ConstantDouble(v),
                _ => new FloatConvert(size, new ConstantDouble(v))
            },
            v => v is IRealValue r
                ? new RealConvert(size, r)
                : new FloatConvert(size, v));
    }
}
