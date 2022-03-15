using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatCeilingTests
{
    private static readonly IContext Context = ContextProxy.Create();

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue value0,
        IValue value1)
    {
        var result0 = FloatCeiling.Create(value0).AsBitVector(Context).Simplify();
        var result1 = FloatCeiling.Create(value1).AsBitVector(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue value0,
        IValue value1)
    {
        var result0 = FloatCeiling.Create(value0).AsBool(Context).Simplify();
        var result1 = FloatCeiling.Create(value1).AsBool(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleUnaryTestData))]
    [ClassData(typeof(DoubleUnaryTestData))]
    private void ShouldCreateEquivalentFloats(
        IValue value0,
        IValue value1)
    {
        var result0 = FloatCeiling.Create(value0).AsFloat(Context).Simplify();
        var result1 = FloatCeiling.Create(value1).AsFloat(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
