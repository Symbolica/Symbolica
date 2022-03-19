using System.Numerics;
using FluentAssertions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class XorTests
{
    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var context = PooledContext.Create();

        using var bv0 = Xor.Create(left0, right0).AsBitVector(context);
        using var result0 = bv0.Simplify();

        using var bv1 = Xor.Create(left1, right1).AsBitVector(context);
        using var result1 = bv1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var context = PooledContext.Create();

        using var b0 = Xor.Create(left0, right0).AsBool(context);
        using var result0 = b0.Simplify();

        using var b1 = Xor.Create(left1, right1).AsBool(context);
        using var result1 = b1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToLeftWhenRightIsZero(IValue value)
    {
        using var context = PooledContext.Create();

        var zero = ConstantUnsigned.Create(value.Size, BigInteger.Zero);

        using var xor = Xor.Create(value, zero).AsBitVector(context);
        using var actual = xor.Simplify();

        using var bv = value.AsBitVector(context);
        using var expected = bv.Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToRightWhenLeftIsZero(IValue value)
    {
        using var context = PooledContext.Create();

        var zero = ConstantUnsigned.Create(value.Size, BigInteger.Zero);

        using var xor = Xor.Create(zero, value).AsBitVector(context);
        using var actual = xor.Simplify();

        using var bv = value.AsBitVector(context);
        using var expected = bv.Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToNotLeftWhenRightIsOnes(IValue value)
    {
        using var context = PooledContext.Create();

        var ones = ConstantUnsigned.Create(value.Size, BigInteger.Zero).Not();

        using var xor = Xor.Create(value, ones).AsBitVector(context);
        using var actual = xor.Simplify();

        using var notBv = Not.Create(value).AsBitVector(context);
        using var expected = notBv.Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToNotRightWhenLeftIsOnes(IValue value)
    {
        using var context = PooledContext.Create();

        var ones = ConstantUnsigned.Create(value.Size, BigInteger.Zero).Not();

        using var xor = Xor.Create(ones, value).AsBitVector(context);
        using var actual = xor.Simplify();

        using var notBv = Not.Create(value).AsBitVector(context);
        using var expected = notBv.Simplify();

        actual.Should().BeEquivalentTo(expected);
    }
}
