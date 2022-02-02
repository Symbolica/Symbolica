using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values;

internal static class FloatOrdered
{
    public static IValue Create(IValue left, IValue right)
    {
        return Value.Create(left, right,
            (l, r) => new ConstantBool(!(float.IsNaN(l) || float.IsNaN(r))),
            (l, r) => new ConstantBool(!(double.IsNaN(l) || double.IsNaN(r))),
            (l, r) => Not.Create(FloatUnordered.Create(l, r)));
    }
}
