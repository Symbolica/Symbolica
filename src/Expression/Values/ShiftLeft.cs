namespace Symbolica.Expression.Values;

public sealed record ShiftLeft : IBinaryBitVectorExpression
{
    private ShiftLeft(IExpression<IType> left, IExpression<IType> right)
    {
        Left = left;
        Right = right;
        Type = new BitVector(left.Size);
    }

    public IExpression<IType> Left { get; }

    public IExpression<IType> Right { get; }

    public BitVector Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as ShiftLeft);
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

    public T Map<T>(IBitVectorMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression<IType> Create(IExpression<IType> left, IExpression<IType> right)
    {
        if (left.Size != right.Size)
            throw new InconsistentExpressionSizesException(left.Size, right.Size);

        return (left, right) switch
        {
            (IConstantValue<IType> l, IConstantValue<IType> r) => l.AsUnsigned().ShiftLeft(r.AsUnsigned()),
            (_, IConstantValue<IType> r) when r.AsUnsigned().IsZero => left,
            (IConstantValue<IType> l, _) when l.AsUnsigned().IsZero => l,
            _ => new ShiftLeft(left, right)
        };
    }
}
