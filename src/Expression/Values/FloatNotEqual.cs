namespace Symbolica.Expression.Values;

public static class FloatNotEqual
{
    public static IExpression<IType> Create(IExpression<IType> left, IExpression<IType> right)
    {
        return LogicalNot.Create(FloatEqual.Create(left, right));
    }
}
