namespace Symbolica.Expression.Values;

public sealed record LogicalNot : Bool, IUnaryExpr
{
    private LogicalNot(IExpression value)
    {
        Value = value;
    }

    public IExpression Value { get; }

    public override bool Equals(IExpression? other)
    {
        return Equals(other as LogicalNot);
    }

    public override T Map<T>(IExprMapper<T> mapper)
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
            IConstantValue v => v.AsBool().Not(),
            LogicalNot v => v.Value,
            Logical v => new LogicalNot(v),
            _ => new LogicalNot(new Logical(value))
        };
    }
}
