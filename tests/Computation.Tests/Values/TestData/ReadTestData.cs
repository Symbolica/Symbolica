using System.Collections.Generic;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class ReadTestData : TheoryData<Bits,
    IConstantValue, IConstantValue,
    SymbolicUnsigned, SymbolicUnsigned>
{
    public ReadTestData()
    {
        foreach (var buffer in Buffers())
        foreach (var offset in Offsets())
        foreach (var size in Sizes())
            Add(size,
                buffer, offset,
                new SymbolicUnsigned(buffer), new SymbolicUnsigned(offset));
    }

    private static IEnumerable<ConstantUnsigned> Buffers()
    {
        yield return ConstantUnsigned.Create((Bits) 32U, int.MinValue);
        yield return ConstantUnsigned.Create((Bits) 32U, uint.MinValue);
        yield return ConstantUnsigned.Create((Bits) 32U, int.MaxValue);
        yield return ConstantUnsigned.Create((Bits) 32U, uint.MaxValue);
    }

    private static IEnumerable<ConstantUnsigned> Offsets()
    {
        yield return ConstantUnsigned.Create((Bits) 32U, 0U);
        yield return ConstantUnsigned.Create((Bits) 32U, 8U);
        yield return ConstantUnsigned.Create((Bits) 32U, 16U);
    }

    private static IEnumerable<Bits> Sizes()
    {
        yield return (Bits) 8U;
        yield return (Bits) 16U;
    }
}
