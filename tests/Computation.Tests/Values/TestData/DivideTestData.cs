using System;
using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class DivideTestData : TheoryData<
    IValue, IValue,
    IValue, IValue>
{
    public DivideTestData()
    {
        var types = new Func<ConstantUnsigned, IValue>[]
        {
            v => v,
            v => new SymbolicUnsigned(v)
        };

        foreach (var left in Values())
        foreach (var right in Values())
        foreach (var left0 in types)
        foreach (var right0 in types)
        foreach (var left1 in types)
        foreach (var right1 in types)
            if (!right.IsZero)
                Add(left0(left), right0(right),
                    left1(left), right1(right));
    }

    private static IEnumerable<ConstantUnsigned> Values()
    {
        return Enumerable.Range(-8, 24).Select(v => ConstantUnsigned.Create(Size.FromBits(4U), v));
    }
}
