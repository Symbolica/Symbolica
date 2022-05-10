using System.Numerics;

namespace Symbolica.Computation;

internal sealed record Range(IValue Min, IValue Max)
{
    internal bool IsDisjoint(Range other)
    {
        return Min is IConstantValue min1
            && Max is IConstantValue max1
            && other.Min is IConstantValue min2
            && other.Max is IConstantValue max2
            && AreConstantRangesDisjoint((min1, max1), (min2, max2));
    }

    internal Range? TryMergeRange(Range other)
    {
        return Min is IConstantValue min1 && Max is IConstantValue max1 && other.Min is IConstantValue min2 && other.Max is IConstantValue max2
            ? TryMergeConstantRanges((min1, max1), (min2, max2))
            : null;
    }

    private static bool AreConstantRangesDisjoint((IConstantValue min, IConstantValue max) a, (IConstantValue min, IConstantValue max) b)
    {
        var maxMin = (BigInteger) a.min.AsUnsigned() < b.min.AsUnsigned()
            ? b.min
            : a.min;

        var minMax = (BigInteger) a.max.AsUnsigned() > b.max.AsUnsigned()
            ? b.max
            : a.max;

        return (BigInteger) minMax.AsUnsigned() < maxMin.AsUnsigned();
    }

    private static Range? TryMergeConstantRanges((IConstantValue min, IConstantValue max) a, (IConstantValue min, IConstantValue max) b)
    {
        var (min, maxMin) = (BigInteger) a.min.AsUnsigned() < (BigInteger) b.min.AsUnsigned()
            ? (a.min, b.min)
            : (b.min, a.min);

        var (max, minMax) = (BigInteger) a.max.AsUnsigned() > (BigInteger) b.max.AsUnsigned()
            ? (a.max, b.max)
            : (b.max, a.max);

        return ((BigInteger) minMax.AsUnsigned() + 1) >= maxMin.AsUnsigned()
            ? (new(min, max))
            : null;
    }
}

