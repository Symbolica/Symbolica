using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatNegateTests
{
    private static readonly Context<ContextHandle> Context = new();

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentConstants(
        IValue value0,
        IValue value1)
    {
        var result0 = FloatNegate.Create(value0).AsConstant(Context);
        var result1 = FloatNegate.Create(value1).AsConstant(Context);

        result0.Should().Be(result1);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue value0,
        IValue value1)
    {
        var result0 = FloatNegate.Create(value0).AsBitVector(Context).Simplify();
        var result1 = FloatNegate.Create(value1).AsBitVector(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue value0,
        IValue value1)
    {
        var result0 = FloatNegate.Create(value0).AsBool(Context).Simplify();
        var result1 = FloatNegate.Create(value1).AsBool(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentFloats(
        IValue value0,
        IValue value1)
    {
        var result0 = FloatNegate.Create(value0).AsFloat(Context).Simplify();
        var result1 = FloatNegate.Create(value1).AsFloat(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
