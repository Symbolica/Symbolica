using System.Collections.Generic;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class WriteTestData : TheoryData<
    IConstantValue, IConstantValue, IConstantValue,
    SymbolicUnsigned, SymbolicUnsigned, SymbolicUnsigned>
{
    public WriteTestData()
    {
        foreach (var buffer in Buffers())
        foreach (var offset in Offsets())
        foreach (var value in Values())
            Add(buffer, offset, value,
                new SymbolicUnsigned(buffer), new SymbolicUnsigned(offset), new SymbolicUnsigned(value));
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

    private static IEnumerable<ConstantUnsigned> Values()
    {
        yield return ConstantUnsigned.Create((Bits) 16U, short.MinValue);
        yield return ConstantUnsigned.Create((Bits) 16U, ushort.MinValue);
        yield return ConstantUnsigned.Create((Bits) 16U, short.MaxValue);
        yield return ConstantUnsigned.Create((Bits) 16U, ushort.MaxValue);
    }
}
