using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record And : IBinaryBitVectorExpression
{
    private And(IExpression<IType> left, IExpression<IType> right)
    {
        Left = left;
        Right = right;
        Type = new BitVector(left.Size);
    }

    public IExpression<IType> Left { get; }

    public IExpression<IType> Right { get; }

    public BitVector Type { get; }

    IInteger IExpression<IInteger>.Type => Type;

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as And);
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

    public T Map<T>(IBitVectorMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    private static IExpression<IType> ShortCircuit(IExpression<IType> left, ConstantUnsigned right)
    {
        return right.IsZero
            ? right
            : right.Not().IsZero
                ? left
                : Create(left, right);
    }

    private static IExpression<IType> Create(IExpression<IType> left, ConstantUnsigned right)
    {
        return left switch
        {
            IConstantValue<IType> l => l.AsUnsigned().And(right),
            And l => Create(l.Left, Create(l.Right, right)),
            _ => new And(left, right)
        };
    }

    public static IExpression<IType> Create(IExpression<IType> left, IExpression<IType> right)
    {
        if (left.Size != right.Size)
            throw new InconsistentExpressionSizesException(left.Size, right.Size);

        return (left, right) switch
        {
            (Bool, _) => LogicalAnd.Create(left, right),
            (_, Bool) => LogicalAnd.Create(left, right),
            (IConstantValue<IType> l, _) => ShortCircuit(right, l.AsUnsigned()),
            (_, IConstantValue<IType> r) => ShortCircuit(left, r.AsUnsigned()),
            _ => new And(left, right)
        };
    }
}
