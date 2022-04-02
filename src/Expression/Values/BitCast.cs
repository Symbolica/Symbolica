namespace Symbolica.Expression.Values;

public static class BitCast
{
    public static IExpression<IType> Create(Bits targetSize, IExpression<IType> expression)
    {
        return expression switch
        {
            Address a => a.BitCast(targetSize),
            _ => expression
        };
    }
}
