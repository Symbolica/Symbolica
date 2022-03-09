using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatSubtractTests
{
    private static readonly IContext Context = ContextFactory.Create();

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentConstants(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var result0 = FloatSubtract.Create(left0, right0).AsConstant(Context);
        var result1 = FloatSubtract.Create(left1, right1).AsConstant(Context);

        result0.Should().Be(result1);
    }

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var result0 = FloatSubtract.Create(left0, right0).AsBitVector(Context).Simplify();
        var result1 = FloatSubtract.Create(left1, right1).AsBitVector(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var result0 = FloatSubtract.Create(left0, right0).AsBool(Context).Simplify();
        var result1 = FloatSubtract.Create(left1, right1).AsBool(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentFloats(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var result0 = FloatSubtract.Create(left0, right0).AsFloat(Context).Simplify();
        var result1 = FloatSubtract.Create(left1, right1).AsFloat(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
