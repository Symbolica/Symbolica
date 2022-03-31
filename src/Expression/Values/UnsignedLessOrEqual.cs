namespace Symbolica.Expression.Values;

public sealed record UnsignedLessOrEqual : Bool, IBinaryExpr
{
    private UnsignedLessOrEqual(IExpression left, IExpression right)
    {
        Left = left;
        Right = right;
    }

    public IExpression Left { get; }

    public IExpression Right { get; }

    public override bool Equals(IExpression? other)
    {
        return Equals(other as UnsignedLessOrEqual);
    }

    public override T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBinaryExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression Create(IExpression left, IExpression right)
    {
        if (left.Size != right.Size)
            throw new InconsistentExpressionSizesException(left.Size, right.Size);

        return (left, right) switch
        {
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().LessOrEqual(r.AsUnsigned()),
            (Address l, _) => Create(l.Aggregate(), right),
            (_, Address r) => Create(left, r.Aggregate()),
            _ => new UnsignedLessOrEqual(left, right)
        };
    }
}
