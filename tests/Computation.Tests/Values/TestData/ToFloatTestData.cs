using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class ToFloatTestData : TheoryData<Size,
    IValue,
    IValue>
{
    public ToFloatTestData()
    {
        var types = new Func<ConstantUnsigned, IValue>[]
        {
            v => v,
            v => new SymbolicUnsigned(v)
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

    private static IEnumerable<ConstantUnsigned> Values()
    {
        return Enumerable.Range(-10, 10).Select(v => ConstantUnsigned.Create(Size.FromBits(32U), v));
    }
}
