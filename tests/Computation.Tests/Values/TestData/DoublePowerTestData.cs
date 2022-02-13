using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class DoublePowerTestData : TheoryData<
    IValue, IValue,
    IValue, IValue>
{
    public DoublePowerTestData()
    {
        var types = new Func<double, IValue>[]
        {
            v => new ConstantDouble(v),
            v => new SymbolicDouble(v)
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

    private static IEnumerable<double> Values()
    {
        return Enumerable.Range(1, 10).Select(i => (double) i);
    }
}
