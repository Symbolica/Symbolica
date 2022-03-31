using Symbolica.Expression;

namespace Symbolica.Computation.Values;

internal static class Resize
{
    public static IValue Create(Bits size, IValue value)
    {
        return Truncate.Create(size, ZeroExtend.Create(size, value));
    }
}
