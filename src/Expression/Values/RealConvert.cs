namespace Symbolica.Expression.Values;

public sealed record RealConvert : IUnaryRealExpression<Real>
{
    public RealConvert(Bits size, IExpression<Real> value)
    {
        Type = new Real(size);
        Value = value;
    }

    public Real Type { get; }

    public IExpression<Real> Value { get; }

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as RealConvert);
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

    public T Map<T>(IRealMapper<T> mapper)
    {
        return mapper.Map(this);
    }
}
