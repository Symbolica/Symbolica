namespace Symbolica.Expression.Values;

public sealed record Truncate : IBitVector, IUnaryExpr
{
    private Truncate(Bits size, IExpression value)
    {
        Size = size;
        Value = value;
    }

    public Bits Size { get; }

    public IExpression Value { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as Truncate);
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
        return size < value.Size
            ? value is IConstantValue v
                ? v.AsUnsigned().Truncate(size)
                : new Truncate(size, value)
            : value;
    }
}
