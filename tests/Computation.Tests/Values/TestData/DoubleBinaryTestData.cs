using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class DoubleBinaryTestData : TheoryData<
    IValue, IValue,
    IValue, IValue>
{
    public DoubleBinaryTestData()
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
        yield return 0d;
        yield return double.Epsilon;
        yield return double.NaN;
        yield return double.NegativeInfinity;
        yield return double.MinValue;
        yield return double.MaxValue;
        yield return double.PositiveInfinity;

        foreach (var i in Enumerable.Range(-10, 10))
        {
            yield return i / 3d;
            yield return 3d / i;
        }

        foreach (var i in Enumerable.Range(1, 10))
        {
            yield return i / 3d;
            yield return 3d / i;
        }
    }
}
