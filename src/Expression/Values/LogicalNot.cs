namespace Symbolica.Expression.Values;

public sealed record LogicalNot : IUnaryBoolExpression
{
    private LogicalNot(IExpression<IType> value)
    {
        Type = Bool.Type;
        Value = value;
    }

    public IExpression<IType> Value { get; }

    public Bool Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as LogicalNot);
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

    public U Map<U>(IIntegerMapper<U> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBoolMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression<IType> Create(IExpression<IType> value)
    {
        return value switch
        {
            IConstantValue<IType> v => v.AsBool().Not(),
            LogicalNot v => v.Value,
            Logical v => new LogicalNot(v),
            _ => new LogicalNot(new Logical(value))
        };
    }
}
