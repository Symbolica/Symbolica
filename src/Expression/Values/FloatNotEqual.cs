namespace Symbolica.Expression.Values;

public static class FloatNotEqual
{
    public static IExpression Create(IExpression left, IExpression right)
    {
        return LogicalNot.Create(FloatEqual.Create(left, right));
    }
}
