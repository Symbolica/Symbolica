using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class SinglePowerTestData : TheoryData<
    IValue, IValue,
    IValue, IValue>
{
    public SinglePowerTestData()
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
        return Enumerable.Range(1, 5).Select(i => (float) i);
    }
}
