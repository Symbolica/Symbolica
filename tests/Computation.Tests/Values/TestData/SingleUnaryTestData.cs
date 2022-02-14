using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class SingleUnaryTestData : TheoryData<
    IValue,
    IValue>
{
    public SingleUnaryTestData()
    {
        var types = new Func<float, IValue>[]
        {
            v => new ConstantSingle(v),
            v => new SymbolicSingle(v)
        };

        foreach (var value in Values())
        foreach (var value0 in types)
        foreach (var value1 in types)
            Add(value0(value),
                value1(value));
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
