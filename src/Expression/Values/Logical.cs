namespace Symbolica.Expression.Values;

public sealed record Logical : Bool, IUnaryExpr
{
    internal Logical(IExpression value)
    {
        Value = value;
    }

    public IExpression Value { get; }

    public override bool Equals(IExpression? other)
    {
        return Equals(other as Logical);
    }

    public override T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IUnaryExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }
}
