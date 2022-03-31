using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record FloatGreaterOrEqual : Bool, IBinaryExpr
{
    private FloatGreaterOrEqual(IExpression left, IExpression right)
    {
        Left = left;
        Right = right;
    }

    public IExpression Left { get; }

    public IExpression Right { get; }

    public override bool Equals(IExpression? other)
    {
        return Equals(other as FloatGreaterOrEqual);
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

        return IFloat.Binary(left, right,
            (l, r) => new ConstantBool(l >= r),
            (l, r) => new ConstantBool(l >= r),
            (l, r) => new FloatGreaterOrEqual(l, r));
    }
}
