namespace Symbolica.Computation.Values;

internal static class FloatOrdered
{
    public static IValue Create(IValue left, IValue right)
    {
        return LogicalNot.Create(FloatUnordered.Create(left, right));
    }
}
