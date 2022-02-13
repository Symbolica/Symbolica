using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class ExtendTestData : TheoryData<Bits,
    IValue,
    IValue>
{
    public ExtendTestData()
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

    private static IEnumerable<Bits> Sizes()
    {
        return Enumerable.Range(4, 5).Select(s => (Bits) (uint) s);
    }

    private static IEnumerable<ConstantUnsigned> Values()
    {
        return Enumerable.Range(-8, 24).Select(v => ConstantUnsigned.Create((Bits) 4U, v));
    }
}
