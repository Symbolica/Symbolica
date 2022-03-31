using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record Multiply : IBitVector, IBinaryExpr
{
    private Multiply(IExpression left, IExpression right)
    {
        Left = left;
        Right = right;
    }

    public IExpression Left { get; }

    public IExpression Right { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as Multiply);
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
            ? right
            : right.IsOne
                ? left
                : Create(left, right);
    }

    private static IExpression Create(IExpression left, ConstantUnsigned right)
    {
        return left switch
        {
            IConstantValue l => l.AsUnsigned().Multiply(right),
            Multiply l => Create(l.Left, Create(l.Right, right)),
            Address l => l.Multiply(right),
            _ => new Multiply(left, right)
        };
    }

    public static IExpression Create(IExpression left, IExpression right)
    {
        if (left.Size != right.Size)
            throw new InconsistentExpressionSizesException(left.Size, right.Size);

        return (left, right) switch
        {
            (IConstantValue l, _) => ShortCircuit(right, l.AsUnsigned()),
            (_, IConstantValue r) => ShortCircuit(left, r.AsUnsigned()),
            (Address l, Address r) => Create(r.Aggregate(), l.Aggregate()),
            (Address l, _) => Create(l.Aggregate(), right),
            (_, Address r) => Create(left, r.Aggregate()),
            _ => new Multiply(left, right)
        };
    }
}
