namespace Symbolica.Expression.Values;

public static class NotEqual
{
    public static IExpression Create(IExpression left, IExpression right)
    {
        return LogicalNot.Create(Equal.Create(left, right));
    }
}
