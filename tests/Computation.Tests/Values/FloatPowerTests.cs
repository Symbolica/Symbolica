using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatPowerTests
{
    [Theory]
    [ClassData(typeof(SinglePowerTestData))]
    [ClassData(typeof(DoublePowerTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var context = PooledContext.Create();

        var real0 = FloatPower.Create(left0, right0);
        var real1 = FloatPower.Create(left1, right1);

        var result0 = FloatToSigned.Create(real0.Size, real0).AsBitVector(context).Simplify();
        var result1 = FloatToSigned.Create(real1.Size, real1).AsBitVector(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SinglePowerTestData))]
    [ClassData(typeof(DoublePowerTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var context = PooledContext.Create();

        var real0 = FloatPower.Create(left0, right0);
        var real1 = FloatPower.Create(left1, right1);

        var result0 = FloatToSigned.Create(real0.Size, real0).AsBool(context).Simplify();
        var result1 = FloatToSigned.Create(real1.Size, real1).AsBool(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SinglePowerTestData))]
    [ClassData(typeof(DoublePowerTestData))]
    private void ShouldCreateEquivalentFloats(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var context = PooledContext.Create();

        var real0 = FloatPower.Create(left0, right0);
        var real1 = FloatPower.Create(left1, right1);

        var result0 = FloatToSigned.Create(real0.Size, real0).AsFloat(context).Simplify();
        var result1 = FloatToSigned.Create(real1.Size, real1).AsFloat(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
