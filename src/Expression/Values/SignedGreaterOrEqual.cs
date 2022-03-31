namespace Symbolica.Expression.Values;

public sealed record SignedGreaterOrEqual : Bool, IBinaryExpr
{
    private SignedGreaterOrEqual(IExpression left, IExpression right)
    {
        Left = left;
        Right = right;
    }

    public IExpression Left { get; }

    public IExpression Right { get; }

    public override bool Equals(IExpression? other)
    {
        return Equals(other as SignedGreaterOrEqual);
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

        return left is IConstantValue l && right is IConstantValue r
            ? l.AsSigned().GreaterOrEqual(r.AsSigned())
            : new SignedGreaterOrEqual(left, right);
    }
}
