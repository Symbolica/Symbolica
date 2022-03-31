using Symbolica.Expression.Values.Constants;

namespace Symbolica.Expression.Values;

public sealed record FloatUnordered : Bool, IBinaryExpr
{
    private FloatUnordered(IExpression left, IExpression right)
    {
        Left = left;
        Right = right;
    }

    public IExpression Left { get; }

    public IExpression Right { get; }

    public override bool Equals(IExpression? other)
    {
        return Equals(other as FloatUnordered);
    }

    public override T Map<T>(IExprMapper<T> mapper)
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
            (l, r) => new ConstantBool(float.IsNaN(l) || float.IsNaN(r)),
            (l, r) => new ConstantBool(double.IsNaN(l) || double.IsNaN(r)),
            (l, r) => new FloatUnordered(l, r));
    }
}
