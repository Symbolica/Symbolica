namespace Symbolica.Expression.Values;

public static class FloatOrdered
{
    public static IExpression<IType> Create(IExpression<IType> left, IExpression<IType> right)
    {
        return LogicalNot.Create(FloatUnordered.Create(left, right));
    }
}
