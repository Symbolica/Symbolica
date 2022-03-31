using Symbolica.Expression;

namespace Symbolica.Expression.Values;

public static class Resize
{
    public static IExpression Create(Bits size, IExpression value)
    {
        return Truncate.Create(size, ZeroExtend.Create(size, value));
    }
}
