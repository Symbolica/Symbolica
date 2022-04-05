using System;
using System.Collections.Generic;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class WriteTestData : TheoryData<
    IValue, IValue, IValue,
    IValue, IValue, IValue>
{
    public WriteTestData()
    {
        var types = new Func<ConstantUnsigned, IValue>[]
        {
            v => v,
            v => new SymbolicUnsigned(v)
        };

        foreach (var buffer in Buffers())
        foreach (var offset in Offsets())
        foreach (var value in Values())
        foreach (var buffer0 in types)
        foreach (var offset0 in types)
        foreach (var buffer1 in types)
        foreach (var offset1 in types)
        foreach (var value0 in types)
        foreach (var value1 in types)
            Add(buffer0(buffer), offset0(offset), value0(value),
                buffer1(buffer), offset1(offset), value1(value));
    }

    private static IEnumerable<ConstantUnsigned> Buffers()
    {
        yield return ConstantUnsigned.Create(Size.FromBits(32U), int.MinValue);
        yield return ConstantUnsigned.Create(Size.FromBits(32U), uint.MinValue);
        yield return ConstantUnsigned.Create(Size.FromBits(32U), int.MaxValue);
        yield return ConstantUnsigned.Create(Size.FromBits(32U), uint.MaxValue);
    }

    private static IEnumerable<ConstantUnsigned> Offsets()
    {
        yield return ConstantUnsigned.Create(Size.FromBits(32U), 0U);
        yield return ConstantUnsigned.Create(Size.FromBits(32U), 8U);
        yield return ConstantUnsigned.Create(Size.FromBits(32U), 16U);
    }

    private static IEnumerable<ConstantUnsigned> Values()
    {
        yield return ConstantUnsigned.Create(Size.FromBits(16U), short.MinValue);
        yield return ConstantUnsigned.Create(Size.FromBits(16U), ushort.MinValue);
        yield return ConstantUnsigned.Create(Size.FromBits(16U), short.MaxValue);
        yield return ConstantUnsigned.Create(Size.FromBits(16U), ushort.MaxValue);
    }
}
