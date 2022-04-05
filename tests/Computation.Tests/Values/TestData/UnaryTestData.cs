using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class UnaryTestData : TheoryData<
    IValue,
    IValue>
{
    public UnaryTestData()
    {
        var types = new Func<ConstantUnsigned, IValue>[]
        {
            v => v,
            v => new SymbolicUnsigned(v)
        };

        foreach (var value in Values())
        foreach (var value0 in types)
        foreach (var value1 in types)
            Add(value0(value),
                value1(value));
    }

    private static IEnumerable<ConstantUnsigned> Values()
    {
        return Enumerable.Range(-8, 24).Select(v => ConstantUnsigned.Create(Size.FromBits(4U), v));
    }
}
