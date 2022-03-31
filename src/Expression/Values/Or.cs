using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record Or : IBitVector, IBinaryExpr
{
    private Or(IExpression left, IExpression right)
    {
        Left = left;
        Right = right;
    }

    public IExpression Left { get; }

    public IExpression Right { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as Or);
    }

    public T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBinaryExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    private static IExpression ShortCircuit(IExpression left, ConstantUnsigned right)
    {
        return right.IsZero
            ? left
            : right.Not().IsZero
                ? right
                : Create(left, right);
    }

    private static IExpression Create(IExpression left, ConstantUnsigned right)
    {
        return left switch
        {
            IConstantValue l => l.AsUnsigned().Or(right),
            Or l => Create(l.Left, Create(l.Right, right)),
            _ => new Or(left, right)
        };
    }

    public static IExpression Create(IExpression left, IExpression right)
    {
        if (left.Size != right.Size)
            throw new InconsistentExpressionSizesException(left.Size, right.Size);

        return (left, right) switch
        {
            (Bool, _) => LogicalOr.Create(left, right),
            (_, Bool) => LogicalOr.Create(left, right),
            (IConstantValue l, _) => ShortCircuit(right, l.AsUnsigned()),
            (_, IConstantValue r) => ShortCircuit(left, r.AsUnsigned()),
            _ => new Or(left, right)
        };
    }
}
