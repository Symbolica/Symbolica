using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record FloatNegate : IUnaryFloatExpression
{
    private FloatNegate(IExpression<IType> value)
    {
        Type = new Float(value.Size);
        Value = value;
    }

    public IExpression<IType> Value { get; }

    public Float Type { get; }

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as FloatNegate);
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

    public static IExpression<IType> Create(IExpression<IType> value)
    {
        return Float.Unary(value,
            v => new ConstantSingle(-v),
            v => new ConstantDouble(-v),
            v => new FloatNegate(v));
    }
}
