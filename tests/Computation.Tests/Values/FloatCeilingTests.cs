using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatCeilingTests
{
    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue value0,
        IValue value1)
    {
        using var context = PooledContext.Create();

        using var bv0 = FloatCeiling.Create(value0).AsBitVector(context);
        using var result0 = bv0.Simplify();

        using var bv1 = FloatCeiling.Create(value1).AsBitVector(context);
        using var result1 = bv1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue value0,
        IValue value1)
    {
        using var context = PooledContext.Create();

        using var b0 = FloatCeiling.Create(value0).AsBool(context);
        using var result0 = b0.Simplify();

        using var b1 = FloatCeiling.Create(value1).AsBool(context);
        using var result1 = b1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentFloats(
        IValue value0,
        IValue value1)
    {
        using var context = PooledContext.Create();

        using var f0 = FloatCeiling.Create(value0).AsFloat(context);
        using var result0 = f0.Simplify();

        using var f1 = FloatCeiling.Create(value1).AsFloat(context);
        using var result1 = f1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
