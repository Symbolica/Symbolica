using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class TruncateTestData : TheoryData<Bits,
    IConstantValue,
    SymbolicUnsigned>
{
    public TruncateTestData()
    {
        foreach (var size in Sizes())
        foreach (var value in Values())
            Add(size,
                value,
                new SymbolicUnsigned(value));
    }

    private static IEnumerable<Bits> Sizes()
    {
        return Enumerable.Range(1, 4).Select(s => (Bits) (uint) s);
    }

    private static IEnumerable<ConstantUnsigned> Values()
    {
        return Enumerable.Range(-8, 24).Select(v => ConstantUnsigned.Create((Bits) 4U, v));
    }
}
