namespace Symbolica.Expression.Values;

public static class FloatOrdered
{
    public static IExpression Create(IExpression left, IExpression right)
    {
        return LogicalNot.Create(FloatUnordered.Create(left, right));
    }
}
