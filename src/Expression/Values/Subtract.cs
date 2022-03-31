using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record Subtract : IBitVector, IBinaryExpr
{
    private Subtract(IExpression left, IExpression right)
    {
        Left = left;
        Right = right;
    }

    public IExpression Left { get; }

    public IExpression Right { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as Subtract);
    }

    public T Map<T>(IExprMapper<T> mapper)
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
            (_, IConstantValue r) when r.AsUnsigned().IsZero => left,
            (IConstantValue l, IConstantValue r) => l.AsUnsigned().Subtract(r.AsUnsigned()),
            (Address l, IConstantValue r) => l.Subtract(r),
            (IConstantValue l, Address r) => r.Negate().Add(l),
            (Address l, _) => Create(l.Aggregate(), right),
            (_, Address r) => Create(left, r.Aggregate()),
            _ when left.Equals(right) => ConstantUnsigned.CreateZero(left.Size),
            _ => new Subtract(left, right)
        };
    }
}
