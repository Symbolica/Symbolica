using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record LogicalAnd : Bool, IBinaryExpr
{
    private LogicalAnd(IExpression left, IExpression right)
    {
        Left = left;
        Right = right;
    }

    public IExpression Left { get; }

    public IExpression Right { get; }

    public override bool Equals(IExpression? other)
    {
        return Equals(other as LogicalAnd);
    }

    public override T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBinaryExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    private static IExpression ShortCircuit(IExpression left, ConstantBool right)
    {
        return right
            ? LogicalNot.Create(LogicalNot.Create(left))
            : right;
    }

    public static IExpression Create(IExpression left, IExpression right)
    {
        if (left.Size != right.Size)
            throw new InconsistentExpressionSizesException(left.Size, right.Size);

        return (left, right) switch
        {
            (_, IConstantValue r) => ShortCircuit(left, r.AsBool()),
            (IConstantValue l, _) => ShortCircuit(right, l.AsBool()),
            _ => new LogicalAnd(left, right)
        };
    }
}
