namespace Symbolica.Expression.Values;

public sealed record SignExtend : IUnaryBitVectorExpression
{
    private SignExtend(Bits size, IExpression<IType> value)
    {
        Type = new BitVector(size);
        Value = value;
    }

    public IExpression<IType> Value { get; }

    public BitVector Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as SignExtend);
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

    public static IExpression<IType> Create(Bits size, IExpression<IType> value)
    {
        return size > value.Size
            ? value is IConstantValue<IType> v
                ? v.AsSigned().Extend(size)
                : new SignExtend(size, value)
            : value;
    }
}
