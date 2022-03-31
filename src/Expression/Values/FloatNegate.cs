using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record FloatNegate : IUnaryExpr, IFloat
{
    private FloatNegate(IExpression value)
    {
        Value = value;
    }

    public IExpression Value { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as FloatNegate);
    }

    public T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IUnaryExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression Create(IExpression value)
    {
        return IFloat.Unary(value,
            v => new ConstantSingle(-v),
            v => new ConstantDouble(-v),
            v => new FloatNegate(v));
    }
}
