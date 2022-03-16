using System.Numerics;
using FluentAssertions;
using Symbolica.Computation.Values.Constants;
using Symbolica.Computation.Values.TestData;
using Xunit;

namespace Symbolica.Computation.Values;

public class XorTests
{
    private static readonly IContext Context = PooledContext.Create();

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBitVectors(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var result0 = Xor.Create(left0, right0).AsBitVector(Context).Simplify();
        var result1 = Xor.Create(left1, right1).AsBitVector(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        var result0 = Xor.Create(left0, right0).AsBool(Context).Simplify();
        var result1 = Xor.Create(left1, right1).AsBool(Context).Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToLeftWhenRightIsZero(IValue value)
    {
        var zero = ConstantUnsigned.Create(value.Size, BigInteger.Zero);

        var actual = Xor.Create(value, zero).AsBitVector(Context).Simplify();
        var expected = value.AsBitVector(Context).Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToRightWhenLeftIsZero(IValue value)
    {
        var zero = ConstantUnsigned.Create(value.Size, BigInteger.Zero);

        var actual = Xor.Create(zero, value).AsBitVector(Context).Simplify();
        var expected = value.AsBitVector(Context).Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToNotLeftWhenRightIsOnes(IValue value)
    {
        var ones = ConstantUnsigned.Create(value.Size, BigInteger.Zero).Not();

        var actual = Xor.Create(value, ones).AsBitVector(Context).Simplify();
        var expected = Not.Create(value).AsBitVector(Context).Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToNotRightWhenLeftIsOnes(IValue value)
    {
        var ones = ConstantUnsigned.Create(value.Size, BigInteger.Zero).Not();

        var actual = Xor.Create(ones, value).AsBitVector(Context).Simplify();
        var expected = Not.Create(value).AsBitVector(Context).Simplify();

        actual.Should().BeEquivalentTo(expected);
    }
}
