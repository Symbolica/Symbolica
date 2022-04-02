using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record FloatEqual : IBinaryBoolExpression
{
    private FloatEqual(IExpression<IType> left, IExpression<IType> right)
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
        return Equals(other as FloatEqual);
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

    public static IExpression<IType> Create(IExpression<IType> left, IExpression<IType> right)
    {
        if (left.Size != right.Size)
            throw new InconsistentExpressionSizesException(left.Size, right.Size);

        return Float.Binary(left, right,
            // ReSharper disable CompareOfFloatsByEqualityOperator
            (l, r) => new ConstantBool(l == r),
            (l, r) => new ConstantBool(l == r),
            // ReSharper restore CompareOfFloatsByEqualityOperator
            (l, r) => new FloatEqual(l, r));
    }
}
