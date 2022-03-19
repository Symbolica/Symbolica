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
        using var solver = PooledSolver.Create();

        using var bv0 = And.Create(left0, right0).AsBitVector(solver);
        using var result0 = bv0.Simplify();

        using var bv1 = And.Create(left1, right1).AsBitVector(solver);
        using var result1 = bv1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(BinaryTestData))]
    private void ShouldCreateEquivalentBooleans(
        IValue left0, IValue right0,
        IValue left1, IValue right1)
    {
        using var solver = PooledSolver.Create();

        using var b0 = And.Create(left0, right0).AsBool(solver);
        using var result0 = b0.Simplify();

        using var b1 = And.Create(left1, right1).AsBool(solver);
        using var result1 = b1.Simplify();

        result0.Should().BeEquivalentTo(result1);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToZeroWhenRightIsZero(IValue value)
    {
        using var solver = PooledSolver.Create();

        var zero = ConstantUnsigned.Create(value.Size, BigInteger.Zero);

        using var and = And.Create(value, zero).AsBitVector(solver);
        using var actual = and.Simplify();

        using var zeroBv = zero.AsBitVector(solver);
        using var expected = zeroBv.Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToZeroWhenLeftIsZero(IValue value)
    {
        using var solver = PooledSolver.Create();

        var zero = ConstantUnsigned.Create(value.Size, BigInteger.Zero);

        using var and = And.Create(zero, value).AsBitVector(solver);
        using var actual = and.Simplify();

        using var zeroBv = zero.AsBitVector(solver);
        using var expected = zeroBv.Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToLeftWhenRightIsOnes(IValue value)
    {
        using var solver = PooledSolver.Create();

        var ones = ConstantUnsigned.Create(value.Size, BigInteger.Zero).Not();

        using var and = And.Create(value, ones).AsBitVector(solver);
        using var actual = and.Simplify();

        using var bv = value.AsBitVector(solver);
        using var expected = bv.Simplify();

        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [ClassData(typeof(IdentityTestData))]
    private void ShouldShortCircuitToRightWhenLeftIsOnes(IValue value)
    {
        using var solver = PooledSolver.Create();

        var ones = ConstantUnsigned.Create(value.Size, BigInteger.Zero).Not();

        using var and = And.Create(ones, value).AsBitVector(solver);
        using var actual = and.Simplify();

        using var bv = value.AsBitVector(solver);
        using var expected = bv.Simplify();

        actual.Should().BeEquivalentTo(expected);
    }
}
