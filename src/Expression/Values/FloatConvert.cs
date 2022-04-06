using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record FloatConvert : IUnaryFloatExpression
{
    private FloatConvert(Bits size, IExpression<IType> value)
    {
        Type = new Float(size);
        Value = value;
    }

    public IExpression<IType> Value { get; }

    public Float Type { get; }

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as FloatConvert);
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
        return Float.Unary(value,
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
            v => v is IExpression<Real> r
                ? new RealConvert(size, r)
                : new FloatConvert(size, v));
    }
}
