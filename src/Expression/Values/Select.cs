namespace Symbolica.Expression.Values;

public sealed record Select : IExpression
{
    private Select(IExpression predicate, IExpression trueValue, IExpression falseValue)
    {
        Predicate = predicate;
        TrueValue = trueValue;
        FalseValue = falseValue;
    }

    public Bits Size => TrueValue.Size;

    public IExpression FalseValue { get; }

    public IExpression Predicate { get; }

    public IExpression TrueValue { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as Select);
    }

    public T Map<T>(IExprMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression Create(IExpression predicate, IExpression trueValue, IExpression falseValue)
    {
        if (trueValue.Size != falseValue.Size)
            throw new InconsistentExpressionSizesException(trueValue.Size, falseValue.Size);

        return (predicate, trueValue, falseValue) switch
        {
            (IConstantValue p, _, _) => p.AsBool() ? trueValue : falseValue,
            _ when trueValue.Equals(falseValue) => trueValue,
            _ => new Select(predicate, trueValue, falseValue)
        };
    }
}
