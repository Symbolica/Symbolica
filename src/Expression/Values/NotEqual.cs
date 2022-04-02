namespace Symbolica.Expression.Values;

public static class NotEqual
{
    public static IExpression<IType> Create(IExpression<IType> left, IExpression<IType> right)
    {
        return LogicalNot.Create(Equal.Create(left, right));
    }
}
