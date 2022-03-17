﻿using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Symbolica.Expression;
using Xunit;

namespace Symbolica.Computation.Values;

public class ZeroExtendTests
{
    [Theory]
    [ClassData(typeof(ExtendTestData))]
    private void ShouldCreateEquivalentBitVectors(Bits size,
        IValue value0,
        IValue value1)
    {
        using var context = PooledContext.Create();

        using var bv0 = ZeroExtend.Create(size, value0).AsBitVector(context);
        using var result0 = bv0.Simplify();

        using var bv1 = ZeroExtend.Create(size, value1).AsBitVector(context);
        using var result1 = bv1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(ExtendTestData))]
    private void ShouldCreateEquivalentBooleans(Bits size,
        IValue value0,
        IValue value1)
    {
        using var context = PooledContext.Create();

        using var b0 = ZeroExtend.Create(size, value0).AsBool(context);
        using var result0 = b0.Simplify();

        using var b1 = ZeroExtend.Create(size, value1).AsBool(context);
        using var result1 = b1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
