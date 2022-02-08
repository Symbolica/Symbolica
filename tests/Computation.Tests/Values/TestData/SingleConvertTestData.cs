using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class SingleConvertTestData : TheoryData<Bits,
    IConstantValue,
    SymbolicFloat>
{
    public SingleConvertTestData()
    {
        foreach (var size in Sizes())
        foreach (var value in Values())
            Add(size,
                new ConstantSingle(value),
                SymbolicFloat.Create(value));
    }

    private static IEnumerable<Bits> Sizes()
    {
        yield return (Bits) 16U;
        yield return (Bits) 32U;
        yield return (Bits) 64U;
        yield return (Bits) 80U;
        yield return (Bits) 128U;
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
