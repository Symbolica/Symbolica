namespace Symbolica.Expression.Values;

public sealed record UnsignedLess : IBinaryBoolExpression
{
    private UnsignedLess(IExpression<IType> left, IExpression<IType> right)
    {
        Left = left;
        Right = right;
        Type = Bool.Type;
    }

    public IExpression<IType> Left { get; }

    public IExpression<IType> Right { get; }

    public Bool Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as UnsignedLess);
    }

    public T Map<T>(IArityMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(ITypeMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBinaryMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IIntegerMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBoolMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression<IType> Create(IExpression<IType> left, IExpression<IType> right)
    {
        if (left.Size != right.Size)
            throw new InconsistentExpressionSizesException(left.Size, right.Size);

        return (left, right) switch
        {
            (IConstantValue<IType> l, IConstantValue<IType> r) => l.AsUnsigned().Less(r.AsUnsigned()),
            _ => new UnsignedLess(left, right)
        };
    }
}
