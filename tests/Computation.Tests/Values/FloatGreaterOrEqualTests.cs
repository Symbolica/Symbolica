using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatGreaterOrEqualTests
{
    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var context = PooledContext.Create();

        var result0 = FloatGreaterOrEqual.Create(left0, right0).AsBitVector(context).Simplify();
        var result1 = FloatGreaterOrEqual.Create(left1, right1).AsBitVector(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SingleBinaryTestData))]
    [ClassData(typeof(DoubleBinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var context = PooledContext.Create();

        var result0 = FloatGreaterOrEqual.Create(left0, right0).AsBool(context).Simplify();
        var result1 = FloatGreaterOrEqual.Create(left1, right1).AsBool(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
