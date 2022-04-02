namespace Symbolica.Expression.Values;

public sealed record SignedToFloat : IUnaryFloatExpression
{
    private SignedToFloat(Bits size, IExpression<IType> value)
    {
        Type = new Float(size);
        Value = value;
    }

    public IExpression<IType> Value { get; }

    public Float Type { get; }

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as SignedToFloat);
    }

    public T Map<T>(IArityMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(ITypeMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IUnaryMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IFloatMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression<IType> Create(Bits size, IExpression<IType> value)
    {
        return value is IConstantValue<IType> v
            ? (uint) size switch
            {
                32U => v.AsSigned().ToSingle(),
                64U => v.AsSigned().ToDouble(),
                _ => new SignedToFloat(size, v)
            }
            : new SignedToFloat(size, value);
    }
}
