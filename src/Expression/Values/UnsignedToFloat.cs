namespace Symbolica.Expression.Values;

public sealed record UnsignedToFloat : IUnaryFloatExpression
{
    private UnsignedToFloat(Bits size, IExpression<IType> value)
    {
        Type = new Float(size);
        Value = value;
    }

    public IExpression<IType> Value { get; }

    public Float Type { get; }

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as UnsignedToFloat);
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
                32U => v.AsUnsigned().ToSingle(),
                64U => v.AsUnsigned().ToDouble(),
                _ => new UnsignedToFloat(size, v)
            }
            : new UnsignedToFloat(size, value);
    }
}
