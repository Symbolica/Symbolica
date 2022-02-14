using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class SingleBinaryTestData : TheoryData<
    IValue, IValue,
    IValue, IValue>
{
    public SingleBinaryTestData()
    {
        var types = new Func<float, IValue>[]
        {
            v => new ConstantSingle(v),
            v => new SymbolicSingle(v)
        };

        foreach (var left in Values())
        foreach (var right in Values())
        foreach (var left0 in types)
        foreach (var right0 in types)
        foreach (var left1 in types)
        foreach (var right1 in types)
            Add(left0(left), right0(right),
                left1(left), right1(right));
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
