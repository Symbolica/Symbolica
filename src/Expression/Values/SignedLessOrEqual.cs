namespace Symbolica.Expression.Values;

public sealed record SignedLessOrEqual : Bool, IBinaryExpr
{
    private SignedLessOrEqual(IExpression left, IExpression right)
    {
        Left = left;
        Right = right;
    }

    public IExpression Left { get; }

    public IExpression Right { get; }

    public override bool Equals(IExpression? other)
    {
        return Equals(other as SignedLessOrEqual);
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
            ? l.AsSigned().LessOrEqual(r.AsSigned())
            : new SignedLessOrEqual(left, right);
    }
}
