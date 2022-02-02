using Symbolica.Computation.Values.Constants;

namespace Symbolica.Computation.Values.Symbolics;

internal static class FloatNotEqual
{
    public static IValue Create(IValue left, IValue right)
    {
        return Value.Create(left, right,
            // ReSharper disable CompareOfFloatsByEqualityOperator
            (l, r) => new ConstantBool(l != r),
            (l, r) => new ConstantBool(l != r),
            // ReSharper restore CompareOfFloatsByEqualityOperator
            (l, r) => Not.Create(FloatEqual.Create(l, r)));
    }
}
