using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class SingleToSignedTestData : TheoryData<Bits,
    IValue,
    IValue>
{
    public SingleToSignedTestData()
    {
        var types = new Func<float, IValue>[]
        {
            v => new ConstantSingle(v),
            v => new SymbolicSingle(v)
        };

        foreach (var size in Sizes())
        foreach (var value in Values())
        foreach (var value0 in types)
        foreach (var value1 in types)
            Add(size,
                value0(value),
                value1(value));
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
