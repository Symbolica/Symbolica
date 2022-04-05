using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class DoubleToUnsignedTestData : TheoryData<Size,
    IValue,
    IValue>
{
    public DoubleToUnsignedTestData()
    {
        var types = new Func<double, IValue>[]
        {
            v => new ConstantDouble(v),
            v => new SymbolicDouble(v)
        };

        foreach (var size in Sizes())
        foreach (var value in Values())
        foreach (var value0 in types)
        foreach (var value1 in types)
            Add(size,
                value0(value),
                value1(value));
    }

    private static IEnumerable<Size> Sizes()
    {
        yield return Size.FromBits(16U);
        yield return Size.FromBits(32U);
        yield return Size.FromBits(64U);
        yield return Size.FromBits(80U);
        yield return Size.FromBits(128U);
    }

    private static IEnumerable<double> Values()
    {
        yield return 0d;
        yield return double.Epsilon;

        foreach (var i in Enumerable.Range(1, 10))
        {
            yield return i / 3d;
            yield return 3d / i;
        }
    }
}
