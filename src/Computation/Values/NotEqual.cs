namespace Symbolica.Computation.Values;

internal static class NotEqual
{
    public static IValue Create(IValue left, IValue right)
    {
        return left is IConstantValue l && right is IConstantValue r
            ? l.AsUnsigned().NotEqual(r.AsUnsigned())
            : Not.Create(Equal.Create(left, right));
    }
}
