using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record LogicalXor : IBinaryBoolExpression
{
    public LogicalXor(IExpression<IType> left, IExpression<IType> right)
    {
        Left = left;
        Right = right;
        Type = Bool.Type;
    }

    public IExpression<IType> Left { get; }

    public IExpression<IType> Right { get; }

    public Bool Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as LogicalXor);
    }

    public T Map<T>(IArityMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(ITypeMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBinaryMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IIntegerMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public T Map<T>(IBoolMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    private static IExpression<IType> ShortCircuit(IExpression<IType> left, ConstantBool right)
    {
        return right
            ? LogicalNot.Create(left)
            : LogicalNot.Create(LogicalNot.Create(left));
    }

    public static IExpression<IType> Create(IExpression<IType> left, IExpression<IType> right)
    {
        if (left.Size != right.Size)
            throw new InconsistentExpressionSizesException(left.Size, right.Size);

        return (left, right) switch
        {
            (_, IConstantValue<IType> r) => ShortCircuit(left, r.AsBool()),
            (IConstantValue<IType> l, _) => ShortCircuit(right, l.AsBool()),
            _ => new LogicalXor(left, right)
        };
    }
}
