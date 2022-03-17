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

        using var bv0 = FloatToSigned.Create(real0.Size, real0).AsBitVector(context);
        using var result0 = bv0.Simplify();

        using var bv1 = FloatToSigned.Create(real1.Size, real1).AsBitVector(context);
        using var result1 = bv1.Simplify();

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

        using var b0 = FloatToSigned.Create(real0.Size, real0).AsBool(context);
        using var result0 = b0.Simplify();

        using var b1 = FloatToSigned.Create(real1.Size, real1).AsBool(context);
        using var result1 = b1.Simplify();

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

        using var f0 = FloatToSigned.Create(real0.Size, real0).AsFloat(context);
        using var result0 = f0.Simplify();

        using var f1 = FloatToSigned.Create(real1.Size, real1).AsFloat(context);
        using var result1 = f1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
