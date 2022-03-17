using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatNegateTests
{
    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue value0,
        IValue value1)
    {
        using var context = PooledContext.Create();

        var result0 = FloatNegate.Create(value0).AsBitVector(context).Simplify();
        var result1 = FloatNegate.Create(value1).AsBitVector(context).Simplify();

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

        var result0 = FloatNegate.Create(value0).AsBool(context).Simplify();
        var result1 = FloatNegate.Create(value1).AsBool(context).Simplify();

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

        var result0 = FloatNegate.Create(value0).AsFloat(context).Simplify();
        var result1 = FloatNegate.Create(value1).AsFloat(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
