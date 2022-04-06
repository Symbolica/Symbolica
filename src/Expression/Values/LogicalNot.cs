namespace Symbolica.Expression.Values;

public sealed record LogicalNot : IUnaryBoolExpression<Bool>
{
    private LogicalNot(IExpression<Bool> value)
    {
        Type = Bool.Type;
        Value = value;
    }

    public Bool Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public IExpression<Bool> Value { get; }

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

    public static IExpression<Bool> Create(IExpression<IType> value)
    {
        return value switch
        {
            IConstantValue<IType> v => v.AsBool().Not(),
            LogicalNot v => v.Value,
            Logical v => new LogicalNot(v),
            _ => new LogicalNot(Logical.Create(value))
        };
    }
}
