using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class SingleBinaryTestData : TheoryData<
    IConstantValue, IConstantValue,
    SymbolicFloat, SymbolicFloat>
{
    public SingleBinaryTestData()
    {
        foreach (var left in Values())
        foreach (var right in Values())
            Add(new ConstantSingle(left), new ConstantSingle(right),
                SymbolicFloat.Create(left), SymbolicFloat.Create(right));
    }

    private static IEnumerable<float> Values()
    {
        yield return 0f;
        yield return float.Epsilon;
        yield return float.NaN;
        yield return float.NegativeInfinity;
        yield return float.MinValue;
        yield return float.MaxValue;
        yield return float.PositiveInfinity;

        foreach (var i in Enumerable.Range(-10, 10))
        {
            yield return i / 3f;
            yield return 3f / i;
        }

        foreach (var i in Enumerable.Range(1, 10))
        {
            yield return i / 3f;
            yield return 3f / i;
        }
    }
}
