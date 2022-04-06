namespace Symbolica.Expression.Values;

public sealed record Select : IExpression<IType>
{
    private Select(IExpression<IType> predicate, IExpression<IType> trueValue, IExpression<IType> falseValue)
    {
        Predicate = predicate;
        TrueValue = trueValue;
        FalseValue = falseValue;
    }

    public IType Type => TrueValue.Type;

    public IExpression<IType> FalseValue { get; }

    public IExpression<IType> Predicate { get; }

    public IExpression<IType> TrueValue { get; }

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as Select);
    }

    public T Map<T>(IArityMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(ITypeMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression<IType> Create(IExpression<IType> predicate, IExpression<IType> trueValue, IExpression<IType> falseValue)
    {
        if (trueValue.Size != falseValue.Size)
            throw new InconsistentExpressionSizesException(trueValue.Size, falseValue.Size);

        return (predicate, trueValue, falseValue) switch
        {
            (IConstantValue<IType> p, _, _) => p.AsBool() ? trueValue : falseValue,
            _ when trueValue.Equals(falseValue) => trueValue,
            _ => new Select(predicate, trueValue, falseValue)
        };
    }
}
