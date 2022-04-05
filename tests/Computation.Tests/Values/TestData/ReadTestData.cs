using System;
using System.Collections.Generic;
using Symbolica.Computation.Values.Constants;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values.TestData;

internal sealed class ReadTestData : TheoryData<Size,
    IValue, IValue,
    IValue, IValue>
{
    public ReadTestData()
    {
        var types = new Func<ConstantUnsigned, IValue>[]
        {
            v => v,
            v => new SymbolicUnsigned(v)
        };

        foreach (var buffer in Buffers())
        foreach (var offset in Offsets())
        foreach (var size in Sizes())
        foreach (var buffer0 in types)
        foreach (var offset0 in types)
        foreach (var buffer1 in types)
        foreach (var offset1 in types)
            Add(size,
                buffer0(buffer), offset0(offset),
                buffer1(buffer), offset1(offset));
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

    private static IEnumerable<Size> Sizes()
    {
        yield return Size.FromBits(8U);
        yield return Size.FromBits(16U);
    }
}
