namespace Symbolica.Computation.Values;

internal static class NotEqual
{
    public static IValue Create(IValue left, IValue right)
    {
        return LogicalNot.Create(Equal.Create(left, right));
    }
}
