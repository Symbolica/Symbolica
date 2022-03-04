namespace Symbolica.Computation.Values;

internal static class FloatNotEqual
{
    public static IValue Create(IValue left, IValue right)
    {
        return LogicalNot.Create(FloatEqual.Create(left, right));
    }
}
