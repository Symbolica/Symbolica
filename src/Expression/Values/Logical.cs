namespace Symbolica.Expression.Values;

public sealed record Logical : IUnaryBoolExpression
{
    private Logical(IExpression<IType> value)
    {
        Type = Bool.Type;
        Value = value;
    }

    public IExpression<IType> Value { get; }

    public Bool Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as Logical);
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
            IConstantValue<IType> v => v.AsBool(),
            IExpression<Bool> v => v,
            _ => new Logical(value)
        };
    }
}
