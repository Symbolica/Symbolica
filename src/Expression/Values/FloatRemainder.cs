using System;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record FloatRemainder : IBinaryFloatExpression
{
    private FloatRemainder(IExpression<IType> left, IExpression<IType> right)
    {
        Left = left;
        Right = right;
        Type = new Float(left.Size);
    }

    public IExpression<IType> Left { get; }

    public IExpression<IType> Right { get; }

    public Float Type { get; }

    public bool Equals(IExpression<IType>? other)
    {
        return Equals(other as FloatRemainder);
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

    public T Map<T>(IFloatMapper<T> mapper)
    {
        return mapper.Map(this);
    }

    public static IExpression<IType> Create(IExpression<IType> left, IExpression<IType> right)
    {
        if (left.Size != right.Size)
            throw new InconsistentExpressionSizesException(left.Size, right.Size);

        return Float.Binary(left, right,
            (l, r) => new ConstantSingle(MathF.IEEERemainder(l, r)),
            (l, r) => new ConstantDouble(Math.IEEERemainder(l, r)),
            (l, r) => new FloatRemainder(l, r));
    }
}
