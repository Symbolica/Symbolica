namespace Symbolica.Expression.Values;

public sealed record UnsignedToFloat : IFloat, IUnaryExpr
{
    private UnsignedToFloat(Bits size, IExpression value)
    {
        Size = size;
        Value = value;
    }

    public Bits Size { get; }

    public IExpression Value { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as UnsignedToFloat);
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
        return value is IConstantValue v
            ? (uint) size switch
            {
                32U => v.AsUnsigned().ToSingle(),
                64U => v.AsUnsigned().ToDouble(),
                _ => new UnsignedToFloat(size, v)
            }
            : new UnsignedToFloat(size, value);
    }
}
