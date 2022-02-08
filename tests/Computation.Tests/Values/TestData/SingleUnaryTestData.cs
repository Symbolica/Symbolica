using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class SingleUnaryTestData : TheoryData<
    IConstantValue,
    SymbolicFloat>
{
    public SingleUnaryTestData()
    {
        foreach (var value in Values())
            Add(new ConstantSingle(value),
                SymbolicFloat.Create(value));
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
