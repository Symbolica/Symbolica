using System.Numerics;
using FluentAssertions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class OrTests
{
    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var context = PooledContext.Create();

        var result0 = Or.Create(left0, right0).AsBitVector(context).Simplify();
        var result1 = Or.Create(left1, right1).AsBitVector(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var context = PooledContext.Create();

        var result0 = Or.Create(left0, right0).AsBool(context).Simplify();
        var result1 = Or.Create(left1, right1).AsBool(context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToLeftWhenRightIsZero(IValue value)
    {
        using var context = PooledContext.Create();

        var zero = ConstantUnsigned.Create(value.Size, BigInteger.Zero);

        var actual = Or.Create(value, zero).AsBitVector(context).Simplify();
        var expected = value.AsBitVector(context).Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToRightWhenLeftIsZero(IValue value)
    {
        using var context = PooledContext.Create();

        var zero = ConstantUnsigned.Create(value.Size, BigInteger.Zero);

        var actual = Or.Create(zero, value).AsBitVector(context).Simplify();
        var expected = value.AsBitVector(context).Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToOnesWhenRightIsOnes(IValue value)
    {
        using var context = PooledContext.Create();

        var ones = ConstantUnsigned.Create(value.Size, BigInteger.Zero).Not();

        var actual = Or.Create(value, ones).AsBitVector(context).Simplify();
        var expected = ones.AsBitVector(context).Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToOnesWhenLeftIsOnes(IValue value)
    {
        using var context = PooledContext.Create();

        var ones = ConstantUnsigned.Create(value.Size, BigInteger.Zero).Not();

        var actual = Or.Create(ones, value).AsBitVector(context).Simplify();
        var expected = ones.AsBitVector(context).Simplify();

        actual.Should().BeEquivalentTo(expected);
    }
}
