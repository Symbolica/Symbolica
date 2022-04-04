namespace Symbolica.Expression.Values;

public static class PointerToInteger
{
    public static IExpression<IType> Create(Bits size, IExpression<IType> expression)
    {
        return Resize.Create(size, expression switch
        {
            Address a => a.ToBitVector(),
            _ => expression
        });
    }
}
