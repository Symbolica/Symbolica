using FluentAssertions;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class FloatPowerTests
{
    private static readonly IContext Context = PooledContext.Create();

    [Theory]
    [ClassData(typeof(SinglePowerTestData))]
    [ClassData(typeof(DoublePowerTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var real0 = FloatPower.Create(left0, right0);
        var real1 = FloatPower.Create(left1, right1);

        var result0 = FloatToSigned.Create(real0.Size, real0).AsBitVector(Context).Simplify();
        var result1 = FloatToSigned.Create(real1.Size, real1).AsBitVector(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SinglePowerTestData))]
    [ClassData(typeof(DoublePowerTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var real0 = FloatPower.Create(left0, right0);
        var real1 = FloatPower.Create(left1, right1);

        var result0 = FloatToSigned.Create(real0.Size, real0).AsBool(Context).Simplify();
        var result1 = FloatToSigned.Create(real1.Size, real1).AsBool(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(SinglePowerTestData))]
    [ClassData(typeof(DoublePowerTestData))]
    private void ShouldCreateEquivalentFloats(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var real0 = FloatPower.Create(left0, right0);
        var real1 = FloatPower.Create(left1, right1);

        var result0 = FloatToSigned.Create(real0.Size, real0).AsFloat(Context).Simplify();
        var result1 = FloatToSigned.Create(real1.Size, real1).AsFloat(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }
}
