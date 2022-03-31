namespace Symbolica.Expression.Values;

public sealed record RealToSigned : IBitVector, IUnaryExpr<IRealValue>
{
    public RealToSigned(Bits size, IRealValue value)
    {
        Size = size;
        Value = value;
    }

    public Bits Size { get; }

    public IRealValue Value { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as RealToSigned);
    }

    public T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IUnaryExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }
}
