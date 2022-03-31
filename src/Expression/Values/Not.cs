namespace Symbolica.Expression.Values;

public sealed record Not : IBitVector, IUnaryExpr
{
    private Not(IExpression value)
    {
        Value = value;
    }

    public IExpression Value { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as Not);
    }

    public T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IUnaryExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression Create(IExpression value)
    {
        return value switch
        {
            IConstantValue v => v.AsUnsigned().Not(),
            Not v => v.Value,
            _ => new Not(value)
        };
    }
}
