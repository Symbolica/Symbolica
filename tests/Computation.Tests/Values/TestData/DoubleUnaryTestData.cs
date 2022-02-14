using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class DoubleUnaryTestData : TheoryData<
    IValue,
    IValue>
{
    public DoubleUnaryTestData()
    {
        var types = new Func<double, IValue>[]
        {
            v => new ConstantDouble(v),
            v => new SymbolicDouble(v)
        };

        foreach (var value in Values())
        foreach (var value0 in types)
        foreach (var value1 in types)
            Add(value0(value),
                value1(value));
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
