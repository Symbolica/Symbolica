using System.Collections.Generic;
using System.Linq;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class ToFloatTestData : TheoryData<Bits,
    IConstantValue,
    SymbolicUnsigned>
{
    public ToFloatTestData()
    {
        foreach (var size in Sizes())
        foreach (var value in Values())
            Add(size,
                value,
                new SymbolicUnsigned(value));
    }

    private static IEnumerable<Bits> Sizes()
    {
        yield return (Bits) 16U;
        yield return (Bits) 32U;
        yield return (Bits) 64U;
        yield return (Bits) 80U;
        yield return (Bits) 128U;
    }

    private static IEnumerable<ConstantUnsigned> Values()
    {
        return Enumerable.Range(-10, 10).Select(v => ConstantUnsigned.Create((Bits) 32U, v));
    }
}
