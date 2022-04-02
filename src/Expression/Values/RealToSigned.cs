namespace Symbolica.Expression.Values;

public sealed record RealToSigned : IUnaryBitVectorExpression<Real>
{
    public RealToSigned(Bits size, IExpression<Real> value)
    {
        Type = new BitVector(size);
        Value = value;
    }

    public BitVector Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public IExpression<Real> Value { get; }


    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as RealToSigned);
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
}
