using System;
using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record FloatPower : IBinaryExpr, IFloat, IRealValue
{
    private FloatPower(IExpression left, IExpression right)
    {
        Left = left;
        Right = right;
    }

    public IExpression Left { get; }

    public IExpression Right { get; }

    public bool Equals(IExpression? other)
    {
        return Equals(other as FloatPower);
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

        return IFloat.Binary(left, right,
            (l, r) => new ConstantSingle(MathF.Pow(l, r)),
            (l, r) => new ConstantDouble(Math.Pow(l, r)),
            (l, r) => new FloatPower(l, r));
    }
}
