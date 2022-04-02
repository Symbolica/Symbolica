namespace Symbolica.Expression.Values;

public static class Resize
{
    public static IExpression<IType> Create(Bits size, IExpression<IType> value)
    {
        return Truncate.Create(size, ZeroExtend.Create(size, value));
    }
}
