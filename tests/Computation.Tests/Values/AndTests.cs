using System.Numerics;
using FluentAssertions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class AndTests
{
    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var context = PooledContext.Create();

        var result0 = And.Create(left0, right0).AsBitVector(context).Simplify();
        var result1 = And.Create(left1, right1).AsBitVector(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var context = PooledContext.Create();

        var result0 = And.Create(left0, right0).AsBool(context).Simplify();
        var result1 = And.Create(left1, right1).AsBool(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToZeroWhenRightIsZero(IValue value)
    {
        using var context = PooledContext.Create();

        var zero = ConstantUnsigned.Create(value.Size, BigInteger.Zero);

        var actual = And.Create(value, zero).AsBitVector(context).Simplify();
        var expected = zero.AsBitVector(context).Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToZeroWhenLeftIsZero(IValue value)
    {
        using var context = PooledContext.Create();

        var zero = ConstantUnsigned.Create(value.Size, BigInteger.Zero);

        var actual = And.Create(zero, value).AsBitVector(context).Simplify();
        var expected = zero.AsBitVector(context).Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToLeftWhenRightIsOnes(IValue value)
    {
        using var context = PooledContext.Create();

        var ones = ConstantUnsigned.Create(value.Size, BigInteger.Zero).Not();

        var actual = And.Create(value, ones).AsBitVector(context).Simplify();
        var expected = value.AsBitVector(context).Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToRightWhenLeftIsOnes(IValue value)
    {
        using var context = PooledContext.Create();

        var ones = ConstantUnsigned.Create(value.Size, BigInteger.Zero).Not();

        var actual = And.Create(ones, value).AsBitVector(context).Simplify();
        var expected = value.AsBitVector(context).Simplify();

        actual.Should().BeEquivalentTo(expected);
    }
}
