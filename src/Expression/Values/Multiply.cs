using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record Multiply : IBinaryBitVectorExpression
{
    private Multiply(IExpression<IType> left, IExpression<IType> right)
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
        return Equals(other as Multiply);
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
            : right.IsOne
                ? left
                : Create(left, right);
    }

    private static IExpression<IType> Create(IExpression<IType> left, ConstantUnsigned right)
    {
        return left switch
        {
            IConstantValue<IType> l => l.AsUnsigned().Multiply(right),
            Multiply l => Create(l.Left, Create(l.Right, right)),
            _ => new Multiply(left, right)
        };
    }

    public static IExpression<IType> Create(IExpression<IType> left, IExpression<IType> right)
    {
        if (left.Size != right.Size)
            throw new InconsistentExpressionSizesException(left.Size, right.Size);

        return (left, right) switch
        {
            (IConstantValue<IType> l, _) => ShortCircuit(right, l.AsUnsigned()),
            (_, IConstantValue<IType> r) => ShortCircuit(left, r.AsUnsigned()),
            _ => new Multiply(left, right)
        };
    }
}
