namespace Symbolica.Expression.Values;

public sealed record Not : IUnaryBitVectorExpression
{
    private Not(IExpression<IType> value)
    {
        Type = new BitVector(value.Size);
        Value = value;
    }

    public IExpression<IType> Value { get; }

    public BitVector Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as Not);
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

    public T Map<T>(IIntegerMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBitVectorMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression<IType> Create(IExpression<IType> value)
    {
        return value switch
        {
            IConstantValue<IType> v => v.AsUnsigned().Not(),
            Not v => v.Value,
            _ => new Not(value)
        };
    }
}
